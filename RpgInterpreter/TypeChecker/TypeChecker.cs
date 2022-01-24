using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreter.TypeChecker.SemanticExceptions;

namespace RpgInterpreter.TypeChecker;

public class TypeChecker
{
    public TypeMap AnalyzeTypes(Root root) => TypeMap.WithBuildIns.EvaluateRoot(root).TypeMap;
}

public record TypeCheckResult<T>(T Type, TypeMap TypeMap) : ITypeCheckResult<T> where T : Type;

public record TypeMap(Scope CurrentScope, IImmutableDictionary<IWithScope, Scope> AllScopes)
{
    public static TypeMap WithBuildIns { get; }

    static TypeMap()
    {
        var builtinTypes = new Type[]
        {
            new UnitType(), new StringType(), new BooleanType(), new IntType(),
            new DiceType()
        }.Select(t => new KeyValuePair<string, Type>(t.ToString(), t));
        var print = new KeyValuePair<string, Type>(
            "print",
            new FunctionType(ImmutableList.Create(new FunctionParameter("value", new StringType())), new UnitType(),
                "print")
        );
        var roll = new KeyValuePair<string, Type>(
            "roll",
            new FunctionType(ImmutableList.Create(new FunctionParameter("value", new DiceType())), new IntType(),
                "roll")
        );
        WithBuildIns = new TypeMap(
            new Scope(ImmutableDictionary.CreateRange(builtinTypes.Append(print).Append(roll))),
            ImmutableDictionary<IWithScope, Scope>.Empty
        );
    }

    public ITypeCheckResult<UnitType> WithUnitType() => new TypeCheckResult<UnitType>(new UnitType(), this);
    public ITypeCheckResult<T> WithType<T>(T type) where T : Type => new TypeCheckResult<T>(type, this);

    public TypeMap WithBaseObjectFields(ObjectType objectType, IPositioned positioned)
    {
        var updated = objectType.FieldTypes.Aggregate(CurrentScope,
            (agg, kvp) =>
            {
                var existing = agg.GetValueOrDefault(kvp.Key);
                if (existing is { } t && !kvp.Value.IsAssignableTo(t))
                {
                    throw new IncompatibleTypesException(t, kvp.Value, positioned);
                }

                return agg.SetItem($"base.{kvp.Key}", kvp.Value);
            });

        return this with { CurrentScope = updated };
    }

    public ITypeCheckResult<UnitType> EvaluateRoot(Root rootNode)
    {
        var state = this;
        foreach (var statement in rootNode.Statements)
        {
            state = state.EvaluateStatement(statement).TypeMap;
        }

        var newScopes = state.AllScopes.Add(rootNode, CurrentScope);

        return (state with { AllScopes = newScopes }).WithUnitType();
    }

    public ITypeCheckResult<UnitType> EvaluateFunctionDeclaration(FunctionDeclaration functionDeclaration)
    {
        var state = this;
        var parameterTypes = new List<FunctionParameter>();
        foreach (var parameter in functionDeclaration.ParameterList.Parameters)
        {
            var parameterType = CurrentScope.Types.GetValueOrDefault(parameter.Type) ??
                                throw new UnrecognizedTypeException(parameter.Name, parameter);
            parameterTypes.Add(new FunctionParameter(parameter.Name, parameterType));
            state = state with
            {
                CurrentScope = state.CurrentScope.Add(parameter.Name, parameterType)
            };
        }

        var returnType = state.CurrentScope.Types.GetValueOrDefault(functionDeclaration.ReturnType)
                         ?? throw new UnrecognizedTypeException(functionDeclaration.ReturnType,
                             new Positioned(functionDeclaration.ParameterList.End, functionDeclaration.Body.Start));

        var functionType = new FunctionType(parameterTypes.ToImmutableList(), returnType, functionDeclaration.Name);

        state = state with { CurrentScope = state.CurrentScope.Add(functionDeclaration.Name, functionType) };

        var bodyResult = state.EvaluateBlock(functionDeclaration.Body);
        var bodyType = bodyResult.Type;
        var bodyMap = bodyResult.TypeMap;

        if (!bodyType.IsAssignableTo(returnType))
        {
            throw new TypeInferenceFailedException(returnType, bodyType, functionDeclaration.Body);
        }

        return new TypeMap
        (
            CurrentScope.Add(functionDeclaration.Name, functionType),
            bodyMap.AllScopes.Add(functionDeclaration, bodyMap.CurrentScope)
        ).WithUnitType();
    }

    public ITypeCheckResult<UnitType> EvaluateFunctionInvocationStatement(FunctionInvocationStatement stmt) =>
        EvaluateFunctionInvocation(stmt.FunctionInvocation).WithUnitType();

    public ITypeCheckResult<Type> EvaluateFunctionInvocation(FunctionInvocation functionInvocation)
    {
        if (functionInvocation.FunctionName == "getString")
        {
            return EvaluateGetString(functionInvocation);
        }

        var function = CurrentScope.Types.GetValueOrDefault(functionInvocation.FunctionName) as FunctionType ??
                       throw new UnrecognizedFunctionNameException(functionInvocation);

        var positions = functionInvocation.Arguments.Select(a => new Positioned(a.Start, a.End)).ToImmutableList();
        var finalTypeMap = this;
        var passedParameters = new List<Type>();
        foreach (var argument in functionInvocation.Arguments)
        {
            var result = finalTypeMap.EvaluateExpression(argument);
            finalTypeMap = result.TypeMap;
            passedParameters.Add(result.Type);
        }

        if (passedParameters.Count != function.ParameterTypes.Count)
        {
            throw new WrongParameterCountException(functionInvocation, function.ParameterTypes.Count);
        }

        for (var i = 0; i < passedParameters.Count; i++)
        {
            var expected = function.ParameterTypes[i].Type;
            var actual = passedParameters[i];

            if (!actual.IsAssignableTo(expected))
            {
                throw new TypeInferenceFailedException(expected, actual, positions[i].Start, positions[i].End);
            }
        }

        return new TypeMap(CurrentScope, finalTypeMap.AllScopes).WithType(
            function.ReturnType);
    }

    public ITypeCheckResult<StringType> EvaluateGetString(FunctionInvocation functionInvocation)
    {
        var parameter = functionInvocation.Arguments.SingleOrDefault() ??
                        throw new WrongParameterCountException(functionInvocation, 1);

        var evaluatedParameter = EvaluateExpression(parameter);

        return evaluatedParameter.WithType(new StringType());
    }

    public ITypeCheckResult<UnitType> EvaluateStatement(Statement statement)
    {
        var newMap = statement switch
        {
            FunctionDeclaration fd => EvaluateFunctionDeclaration(fd),
            ObjectDeclaration od => EvaluateObjectDeclaration(od),
            Assignment a => EvaluateAssignment(a),
            TraitDeclaration td => EvaluateTraitDeclaration(td),
            FunctionInvocationStatement fi => EvaluateFunctionInvocationStatement(fi),
            _ => throw new ArgumentOutOfRangeException(nameof(statement), statement, "Unrecognized statement type.")
        };

        return newMap;
    }

    private ITypeCheckResult<UnitType> EvaluateTraitDeclaration(TraitDeclaration td)
    {
        var typeMap = this;
        var baseTypes = new List<ObjectType>();
        var fieldDict = ImmutableDictionary<string, Type>.Empty;

        if (td.Base is not null)
        {
            var baseType = CurrentScope.Types.GetValueOrDefault(td.Base) as ObjectType ??
                           throw new UnrecognizedTypeException(td.Base, new Positioned(td.Start, td.Fields.Start));
            typeMap = typeMap.WithBaseObjectFields(baseType, td);
            baseTypes.Add(baseType);
            fieldDict = fieldDict.AddRange(baseType.FieldTypes);
        }

        foreach (var field in td.Fields.Fields)
        {
            var name = field.Name;
            var fieldResult = typeMap.EvaluateExpression(field.Value);

            if (typeMap.CurrentScope.Types.ContainsKey($"this.{name}"))
            {
                throw new FieldRedeclarationException(field, td.Name);
            }

            typeMap = fieldResult.TypeMap with
            {
                CurrentScope = typeMap.CurrentScope.Add($"this.{name}", fieldResult.Type)
            };

            if (fieldDict.GetValueOrDefault("name") is { } existingType &&
                !fieldResult.Type.IsAssignableTo(existingType))
            {
                throw new TypeInferenceFailedException(existingType, fieldResult.Type, field);
            }

            fieldDict = fieldDict.SetItem(name, fieldResult.Type);
        }

        var objectType = new ObjectType(fieldDict, td.Name, baseTypes.ToImmutableHashSet());

        return new TypeMap(
            CurrentScope.Add(objectType.TypeName, objectType),
            typeMap.AllScopes.Add(td, typeMap.CurrentScope)
        ).WithUnitType();
    }

    private ITypeCheckResult<UnitType> EvaluateObjectDeclaration(ObjectDeclaration od)
    {
        var typeMap = this;
        var baseTypes = new List<ObjectType>();


        if (od.Base is not null)
        {
            var baseType = CurrentScope.Types.GetValueOrDefault(od.Base) as ObjectType ??
                           throw new UnrecognizedTypeException(od.Base, new Positioned(od.Start, od.Fields.Start));
            typeMap = typeMap.WithBaseObjectFields(baseType, od);
            baseTypes.Add(baseType);
        }

        if (od.Traits is not null)
        {
            foreach (var trait in od.Traits.Traits)
            {
                var traitType = CurrentScope.Types.GetValueOrDefault(trait) as ObjectType ??
                                throw new UnrecognizedTypeException(trait, od.Traits);
                typeMap = typeMap.WithBaseObjectFields(traitType, od);
                baseTypes.Add(traitType);
            }
        }

        var fieldDict = ImmutableDictionary<string, Type>.Empty;

        foreach (var baseType in baseTypes)
        {
            foreach (var (name, type) in baseType.FieldTypes)
            {
                var newType = type;
                if (fieldDict.GetValueOrDefault(name) is { } existingType)
                {
                    if (!newType.IsAssignableTo(existingType) &&
                        !(existingType is TypedListType && newType is EmptyListType))
                    {
                        throw new ConflictingTraitsDeclaration(new Positioned(od.Start, od.Fields.Start), od.Name,
                            existingType, name,
                            baseType.TypeName, newType);
                    }

                    if (existingType is TypedListType && newType is EmptyListType)
                    {
                        newType = existingType;
                    }
                }

                fieldDict = fieldDict.SetItem(name, newType);
            }
        }

        foreach (var field in od.Fields.Fields)
        {
            var name = field.Name;
            var typeResult = typeMap.EvaluateExpression(field.Value);

            if (typeMap.CurrentScope.Types.ContainsKey($"this.{name}"))
            {
                throw new FieldRedeclarationException(field, od.Name);
            }

            typeMap = typeResult.TypeMap with
            {
                CurrentScope = typeMap.CurrentScope.Add($"this.{name}", typeResult.Type)
            };
            fieldDict = fieldDict.Add(name, typeResult.Type);
        }

        var objectType = new ObjectType(fieldDict, od.Name, ImmutableHashSet.CreateRange(baseTypes));

        return new TypeMap(
            CurrentScope.Add(objectType.TypeName, objectType),
            typeMap.AllScopes.Add(od, typeMap.CurrentScope.Add(objectType.TypeName, objectType))
        ).WithUnitType();
    }

    private ITypeCheckResult<UnitType> EvaluateAssignment(Assignment assignment)
    {
        var target = assignment.Target;

        if (target is VariableExp variable)
        {
            var rightSide = EvaluateExpression(assignment.Value);
            if (CurrentScope.Types.ContainsKey(variable.Name))
            {
                var variableType = CurrentScope.Types[variable.Name];
                if (!rightSide.Type.IsAssignableTo(variableType) &&
                    !(variableType is TypedListType && rightSide.Type is EmptyListType))
                {
                    throw new TypeInferenceFailedException(variableType, rightSide.Type, assignment);
                }

                if (variableType is TypedListType && rightSide.Type is EmptyListType)
                {
                    return new TypeMap(CurrentScope, rightSide.TypeMap.AllScopes).WithUnitType();
                }
            }

            return new TypeMap(CurrentScope.SetItem(variable.Name, rightSide.Type), rightSide.TypeMap.AllScopes)
                .WithUnitType();
        }

        if (target is FieldReference field)
        {
            var objectName = field.ObjectName switch
            {
                Base => "base",
                This => "this",
                VariableExp v => v.Name,
                _ => throw new InvalidOperationException()
            };

            var lhsType = CurrentScope.Types.GetValueOrDefault(objectName);
            if (lhsType is null)
            {
                throw new UnassignedVariableException(objectName, field.ObjectName);
            }

            if (lhsType is not ObjectType obj)
            {
                throw new NoSuchFieldException(field, lhsType);
            }

            if (!obj.FieldTypes.ContainsKey(field.FieldName))
            {
                throw new NoSuchFieldException(field, obj);
            }

            var rightSide = EvaluateExpression(assignment.Value);
            if (obj.FieldTypes.ContainsKey(field.FieldName))
            {
                var variableType = obj.FieldTypes[field.FieldName];
                if (!rightSide.Type.IsAssignableTo(variableType) &&
                    !(variableType is TypedListType && rightSide.Type is EmptyListType))
                {
                    throw new TypeInferenceFailedException(variableType, rightSide.Type, assignment);
                }

                if (variableType is TypedListType && rightSide.Type is EmptyListType)
                {
                    return new TypeMap(CurrentScope, rightSide.TypeMap.AllScopes).WithUnitType();
                }
            }

            return new TypeMap(CurrentScope.SetItem(field.FieldName, rightSide.Type), rightSide.TypeMap.AllScopes)
                .WithUnitType();
        }

        return WithUnitType();
    }


    public ITypeCheckResult<ListType> EvaluateList(ListExp list)
    {
        if (list.Elements.Count == 0)
        {
            return WithType(new EmptyListType(list));
        }

        var finalTypeMap = this;
        var values = new List<Type>();
        foreach (var element in list.Elements)
        {
            var result = finalTypeMap.EvaluateExpression(element);
            finalTypeMap = result.TypeMap;
            values.Add(result.Type);
        }

        var type = values.First();

        if (values.Any(v => !v.IsAssignableTo(type)))
        {
            throw new TypeInferenceFailedException(list);
        }

        return finalTypeMap.WithType(new TypedListType(type));
    }

    public ITypeCheckResult<Type> EvaluateExpression(Expression expression)
    {
        var value = expression switch
        {
            DiceExpression => WithType(new DiceType()),
            Natural => WithType(new IntType()),
            BooleanExpression => WithType(new BooleanType()),
            StringExpression => WithType(new StringType()),
            BinaryOperation bo => EvaluateBinaryOperation(bo),
            FunctionInvocation fi => EvaluateFunctionInvocation(fi),
            ListExp le => EvaluateList(le),
            Unit => WithType(new UnitType()),
            UnaryMinus um => EvaluateUnaryMinus(um),
            IfExpression i => EvaluateIf(i),
            Block b => EvaluateBlock(b),
            ObjectCreation oc => EvaluateObjectCreation(oc),
            Reference r => EvaluateReference(r),
            _ => throw new ArgumentOutOfRangeException(nameof(expression), expression, "Unrecognized expression type.")
        };

        return value;
    }


    private ITypeCheckResult<ObjectType> EvaluateObjectCreation(ObjectCreation oc)
    {
        if (CurrentScope.Types.GetValueOrDefault(oc.Type) is not ObjectType baseType)
        {
            throw new UnrecognizedTypeException(oc.Type, oc);
        }

        if (oc.Traits is null)
        {
            return WithType(baseType);
        }

        var anonymousType = new ObjectDeclaration(
            "anonymous",
            oc.Type,
            oc.Traits,
            new FieldList(new NodeList<FieldDeclaration>(Enumerable.Empty<FieldDeclaration>()), oc.End, oc.End),
            oc.Start,
            oc.End
        );

        var withAnonType = EvaluateObjectDeclaration(anonymousType).TypeMap;

        var creation = new ObjectCreation(
            anonymousType.Name,
            null,
            anonymousType.Start,
            anonymousType.End
        );
        return withAnonType.EvaluateObjectCreation(creation);
    }

    private ITypeCheckResult<Type> EvaluateBlock(Block block)
    {
        var blockInner = this;
        blockInner = block.Inner.Aggregate(blockInner, (current, inner) => inner switch
        {
            Expression e => current.EvaluateExpression(e).TypeMap,
            Assignment a => current.EvaluateAssignment(a).TypeMap,
            _ => throw new InvalidOperationException($"{inner} is not a valid statement inside a block.")
        });

        var returnType = blockInner.EvaluateExpression(block.Last);

        return returnType;
    }

    private ITypeCheckResult<Type> EvaluateIf(IfExpression ifExpression)
    {
        var conditionResult = EvaluateExpression(ifExpression.Condition);
        var conditionType = conditionResult.Type;
        if (conditionType is not BooleanType)
        {
            throw new IfConditionMustBeBooleanException(ifExpression, conditionType);
        }

        var trueResult = conditionResult.TypeMap.EvaluateExpression(ifExpression.TrueValue);
        var trueType = trueResult.Type;
        var falseResult = trueResult.TypeMap.EvaluateExpression(ifExpression.FalseValue);
        var falseType = falseResult.Type;

        if (!trueType.IsAssignableTo(falseType) && !falseType.IsAssignableTo(trueType))
        {
            throw new TypeInferenceFailedException(ifExpression);
        }

        return falseResult;
    }

    private ITypeCheckResult<Type> EvaluateUnaryMinus(UnaryMinus um)
    {
        var inner = EvaluateExpression(um.Value);

        return inner.Type switch
        {
            IntType => inner.WithType(new IntType()),
            DiceType => inner.WithType(new DiceType()),
            _ => throw new NonNumericNegationException(um, inner.Type)
        };
    }

    public ITypeCheckResult<Type> EvaluateReference(Reference reference)
    {
        var value = reference switch
        {
            VariableExp v => WithType(
                CurrentScope.Types.GetValueOrDefault(v.Name) ?? throw new UnassignedVariableException(v)),
            Base b => throw new ObjectSelfReferenceIsNotAllowed(b),
            This t => throw new ObjectSelfReferenceIsNotAllowed(t),
            FieldReference f => EvaluateFieldReference(f),
            _ => throw new ArgumentOutOfRangeException(nameof(reference), reference, "Unrecognized reference type.")
        };

        return value;
    }

    public ITypeCheckResult<Type> EvaluateFieldReference(FieldReference reference)
    {
        string? name;
        if (reference.ObjectName is Base)
        {
            name = $"base.{reference.FieldName}";
        }
        else if (reference.ObjectName is This)
        {
            name = $"this.{reference.FieldName}";
        }
        else if (reference.ObjectName is VariableExp v)
        {
            if (CurrentScope.Types.GetValueOrDefault(v.Name) is not ObjectType objectType)
            {
                throw new NotAnObjectException(v.Name, v);
            }

            var fieldType = objectType.FieldTypes.GetValueOrDefault(reference.FieldName)
                            ?? throw new NoSuchFieldException(reference, objectType);

            return WithType(fieldType);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(reference), reference,
                "Unrecognized field reference type.");
        }

        var deducedType = CurrentScope.Types.GetValueOrDefault(name) ??
                          throw new UnassignedVariableException(name, reference);

        return WithType(deducedType);
    }

    public ITypeCheckResult<Type> EvaluateBinaryOperation(BinaryOperation binaryOperation)
    {
        var leftResult = EvaluateExpression(binaryOperation.Left);
        var leftType = leftResult.Type;
        var rightResult = leftResult.TypeMap.EvaluateExpression(binaryOperation.Right);
        var rightType = rightResult.Type;
        var rightMap = rightResult.TypeMap;

        return binaryOperation switch
        {
            AdditionExp => rightMap.EvaluateAddition(leftType, rightType, binaryOperation),
            SubtractionExp => rightMap.EvaluateSubtraction(leftType, rightType, binaryOperation),
            MultiplicationExp => rightMap.EvaluateMultiplication(leftType, rightType, binaryOperation),
            DivisionExp => rightMap.EvaluateDivision(leftType, rightType, binaryOperation),
            BooleanAndExp => rightMap.EvaluateBooleanAnd(leftType, rightType, binaryOperation),
            BooleanOrExp => rightMap.EvaluateBooleanOr(leftType, rightType, binaryOperation),
            ConcatenationExp => rightMap.EvaluateConcatenation(leftType, rightType, binaryOperation),
            EqualExp => rightMap.EvaluateEqual(leftType, rightType, binaryOperation),
            NotEqualExp => rightMap.EvaluateNotEqual(leftType, rightType, binaryOperation),
            LessThanExp => rightMap.EvaluateLessThan(leftType, rightType, binaryOperation),
            GreaterThanExp => rightMap.EvaluateGreaterThan(leftType, rightType, binaryOperation),
            LessOrEqualThanExp => rightMap.EvaluateLessOrEqual(leftType, rightType, binaryOperation),
            GreaterOrEqualThanExp => rightMap.EvaluateGreaterOrEqual(leftType, rightType, binaryOperation),
            _ => throw new ArgumentOutOfRangeException(nameof(binaryOperation), binaryOperation,
                "Unrecognized binary operation type.")
        };
    }

    public ITypeCheckResult<Type> EvaluateNumericOperation(Type left, Type right, IPositioned position,
        [CallerMemberName] string operationName = "")
    {
        return (left, right) switch
        {
            (IntType, IntType) => WithType(new IntType()),
            (DiceType, DiceType) => WithType(new DiceType()),
            (IntType, DiceType) => WithType(new DiceType()),
            (DiceType, IntType) => WithType(new DiceType()),
            _ => throw new InvalidBinaryOperationException(left, right, operationName["Evaluate".Length..], position)
        };
    }

    public ITypeCheckResult<Type> EvaluateAddition(Type left, Type right, IPositioned position) =>
        EvaluateNumericOperation(left, right, position);

    public ITypeCheckResult<Type> EvaluateSubtraction(Type left, Type right, IPositioned position) =>
        EvaluateNumericOperation(left, right, position);

    public ITypeCheckResult<Type> EvaluateMultiplication(Type left, Type right, IPositioned position) =>
        EvaluateNumericOperation(left, right, position);

    public ITypeCheckResult<Type> EvaluateDivision(Type left, Type right, IPositioned position) =>
        EvaluateNumericOperation(left, right, position);

    public ITypeCheckResult<Type> EvaluateBooleanAnd(Type left, Type right, IPositioned position)
    {
        return (left, right) switch
        {
            (BooleanType, BooleanType) => WithType(new BooleanType()),
            _ => throw new InvalidBinaryOperationException(left, right, "boolean and", position)
        };
    }

    public ITypeCheckResult<Type> EvaluateBooleanOr(Type left, Type right, IPositioned position)
    {
        return (left, right) switch
        {
            (BooleanType, BooleanType) => WithType(new BooleanType()),
            _ => throw new InvalidBinaryOperationException(left, right, "boolean or", position)
        };
    }

    public ITypeCheckResult<Type> EvaluateConcatenation(Type left, Type right, IPositioned position)
    {
        return (left, right) switch
        {
            (StringType, StringType) => WithType(new StringType()),
            (TypedListType l, EmptyListType) => WithType(l),
            (EmptyListType, TypedListType r) => WithType(r),
            (TypedListType l, TypedListType r) when l.ElementType == r.ElementType => WithType(l),
            _ => throw new InvalidBinaryOperationException(left, right, "concatenation", position)
        };
    }

    public ITypeCheckResult<Type> EvaluateEqual(Type left, Type right, IPositioned position)
    {
        return (left, right) switch
        {
            (IntType, IntType) => WithType(new BooleanType()),
            (StringType, StringType) => WithType(new BooleanType()),
            _ => throw new InvalidBinaryOperationException(left, right, "equality", position)
        };
    }

    public ITypeCheckResult<Type> EvaluateNotEqual(Type left, Type right, IPositioned position)
    {
        return (left, right) switch
        {
            (IntType, IntType) => WithType(new BooleanType()),
            (StringType, StringType) => WithType(new BooleanType()),
            _ => throw new InvalidBinaryOperationException(left, right, "equality", position)
        };
    }

    public ITypeCheckResult<Type> EvaluateLessThan(Type left, Type right, IPositioned position)
    {
        return (left, right) switch
        {
            (IntType, IntType) => WithType(new BooleanType()),
            _ => throw new InvalidBinaryOperationException(left, right, "comparison", position)
        };
    }

    public ITypeCheckResult<Type> EvaluateGreaterThan(Type left, Type right, IPositioned position)
    {
        return (left, right) switch
        {
            (IntType, IntType) => WithType(new BooleanType()),
            _ => throw new InvalidBinaryOperationException(left, right, "comparison", position)
        };
    }

    public ITypeCheckResult<Type> EvaluateLessOrEqual(Type left, Type right, IPositioned position)
    {
        return (left, right) switch
        {
            (IntType, IntType) => WithType(new BooleanType()),
            _ => throw new InvalidBinaryOperationException(left, right, "comparison", position)
        };
    }

    public ITypeCheckResult<Type> EvaluateGreaterOrEqual(Type left, Type right, IPositioned position)
    {
        return (left, right) switch
        {
            (IntType, IntType) => WithType(new BooleanType()),
            _ => throw new InvalidBinaryOperationException(left, right, "comparison", position)
        };
    }
}