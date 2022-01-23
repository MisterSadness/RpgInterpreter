using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreter.Runtime.SemanticExceptions;
using RpgInterpreter.TypeChecker;
using Type = RpgInterpreter.TypeChecker.Type;

namespace RpgInterpreter.Runtime;

public record ObjectConstructor;

public record InheritingObjectConstructor : ObjectConstructor;

public class TypeCheckShouldHaveFailedException : Exception
{
    public TypeCheckShouldHaveFailedException() : base("Type check should be run before interpretation.") { }
}

public static class TypeExtensions
{
    private static Exception GetException() => new TypeCheckShouldHaveFailedException();

    public static FunctionType AsFunctionType(this Type type) => type as FunctionType ?? throw GetException();
    public static IntType AsIntType(this Type type) => type as IntType ?? throw GetException();
    public static BooleanType AsBooleanType(this Type type) => type as BooleanType ?? throw GetException();
    public static UnitType AsUnitType(this Type type) => type as UnitType ?? throw GetException();
    public static StringType AsStringType(this Type type) => type as StringType ?? throw GetException();
    public static ObjectType AsObjectType(this Type type) => type as ObjectType ?? throw GetException();
    public static ListType AsListType(this Type type) => type as ListType ?? throw GetException();
}

public record InterpreterState(
    IImmutableDictionary<string, Object> Objects,
    IImmutableDictionary<string, Object> Traits,
    IImmutableDictionary<string, Function> Functions,
    IImmutableDictionary<string, Value> Variables,
    Scope CurrentScope,
    IImmutableDictionary<IWithScope, Scope> AllScopes,
    IOutput Output)
{
    public static InterpreterState Initial(TypeMap typeMap) => new(
        ImmutableDictionary<string, Object>.Empty,
        ImmutableDictionary<string, Object>.Empty,
        ImmutableDictionary<string, Function>.Empty.Add(Print.Name, Print),
        ImmutableDictionary<string, Value>.Empty,
        typeMap.CurrentScope,
        typeMap.AllScopes,
        new ConsoleOutput()
    );

    public int AnonymousObjectCount { get; init; }

    private static Exception GetException() => new TypeCheckShouldHaveFailedException();

    public InterpreterState EvaluateRoot(Root rootNode)
    {
        // TODO Handle function, object and trait declarations in some smarter order
        var state = this;
        foreach (var statement in rootNode.Statements)
        {
            state = state.EvaluateStatement(statement);
        }

        return state;
    }
    
    public static Function Print { get; } = new(
        body: state =>
        {
            var stringParam = state.Variables.GetValueOrDefault("value") as String ??
                              throw new InvalidOperationException(
                                  $"Parameters should be already verified in {nameof(EvaluateFunctionInvocation)}");
            state.Output.Write(stringParam.Value);
            return new Unit();
        },
        type: new FunctionType(
            new[] { new FunctionParameter("value", new StringType()) }.ToImmutableList(),
            new UnitType(),
            "print")
    );

    public InterpreterState EvaluateFunctionDeclaration(FunctionDeclaration functionDeclaration)
    {
        var name = functionDeclaration.Name;
        if (Functions.ContainsKey(name))
        {
            throw new FunctionRedeclarationException(functionDeclaration);
        }

        var functionType = CurrentScope.GetTypeOf(functionDeclaration.Name).AsFunctionType();
        var body = new Function.Call(state => state.EvaluateBlock(functionDeclaration.Body));
        var function = new Function(body, functionType);

        return this with { Functions = Functions.Add(name, function) };
    }

    public InterpreterState EvaluateFunctionInvocationStatement(FunctionInvocationStatement functionInvocationStatement)
    {
        var _ = EvaluateFunctionInvocation(functionInvocationStatement.FunctionInvocation);
        return this;
    }

    public Value EvaluateFunctionInvocation(FunctionInvocation functionInvocation)
    {
        if (functionInvocation.FunctionName == "getString")
        {
            return EvaluateGetString(functionInvocation);
        }

        var function = Functions.GetValueOrDefault(functionInvocation.FunctionName) ??
                       throw new UnrecognizedFunctionNameException(functionInvocation);

        var passedParameters = functionInvocation.Arguments.Select(EvaluateExpression).ToImmutableList();

        var dictionary = new Dictionary<string, Value>();

        for (var i = 0; i < passedParameters.Count; i++)
        {
            var expected = function.Parameters[i];
            var actual = passedParameters[i];

            dictionary[expected.Name] = actual;
        }

        return function.Body.Invoke(this with { Variables = Variables.SetItems(dictionary) });
    }

    public String EvaluateGetString(FunctionInvocation functionInvocation)
    {
        var argument = functionInvocation.Arguments.SingleOrDefault() ??
                       throw new WrongParameterCountException(functionInvocation, 1);

        var evaluated = EvaluateExpression(argument);

        return new String(evaluated.PrintableString);
    }

    public InterpreterState EvaluateStatement(Statement statement)
    {
        var newState = statement switch
        {
            FunctionDeclaration fd => EvaluateFunctionDeclaration(fd),
            ObjectDeclaration od => EvaluateObjectDeclaration(od),
            Assignment a => EvaluateAssignment(a),
            TraitDeclaration td => EvaluateTraitDeclaration(td),
            FunctionInvocationStatement fi => EvaluateFunctionInvocationStatement(fi),
            _ => throw new ArgumentOutOfRangeException(nameof(statement), statement, "Unrecognized statement type.")
        };

        return newState;
    }

    private InterpreterState EvaluateTraitDeclaration(TraitDeclaration od)
    {
        var baseFields = ImmutableDictionary<string, Value>.Empty;
        var type = CurrentScope.GetTypeOf(od.Name).AsObjectType();
        var declarationScope = AllScopes[od];
        var state = this with { CurrentScope = declarationScope };

        var resultObject = new Object(baseFields, type);

        // set initial fields from base
        if (od.Base is not null)
        {
            var baseType = state.CurrentScope.GetTypeOf(od.Base).AsObjectType();
            var baseObject = state.Objects[baseType.TypeName];
            baseFields = baseFields.SetItems(baseObject.Fields);
            state = this with { Variables = state.Variables.Add("base", baseObject) };
        }

        // calculate this fields
        var thisFields = ImmutableDictionary<string, Value>.Empty;
        foreach (var field in od.Fields.Fields)
        {
            var name = field.Name;
            var value = state.EvaluateExpression(field.Value);
            thisFields = thisFields.SetItem(name, value);
            resultObject = resultObject with { Fields = thisFields };
            state = state with { Variables = state.Variables.SetItem("this", resultObject) };
        }

        // overwrite base with this fields
        resultObject = resultObject with { Fields = baseFields.SetItems(thisFields) };

        return this with { Traits = Traits.Add(type.TypeName, resultObject) };
    }

    private InterpreterState EvaluateObjectDeclaration(ObjectDeclaration od)
    {
        var baseFields = ImmutableDictionary<string, Value>.Empty;
        var type = CurrentScope.GetTypeOf(od.Name).AsObjectType();
        var declarationScope = AllScopes[od];
        var state = this with { CurrentScope = declarationScope };

        // set initial fields from base
        if (od.Base is not null)
        {
            var baseType = state.CurrentScope.GetTypeOf(od.Base).AsObjectType();
            var baseObject = state.Objects[baseType.TypeName];
            baseFields = baseFields.SetItems(baseObject.Fields);
            state = this with { Variables = state.Variables.Add("base", baseObject) };
        }

        // update base fields with traits
        if (od.Traits is not null)
        {
            foreach (var trait in od.Traits.Traits)
            {
                var traitType = state.CurrentScope.GetTypeOf(trait).AsObjectType();
                var baseObject = state.Traits[traitType.TypeName];
                baseFields = baseFields.SetItems(baseObject.Fields);
                state = this with { Variables = state.Variables.SetItem("base", baseObject) };
            }
        }

        // calculate this fields
        var thisFields = ImmutableDictionary<string, Value>.Empty;
        var resultObject = new Object(thisFields, type);
        foreach (var field in od.Fields.Fields)
        {
            var name = field.Name;
            var value = state.EvaluateExpression(field.Value);
            thisFields = thisFields.SetItem(name, value);
            resultObject = resultObject with { Fields = thisFields };
            state = state with { Variables = state.Variables.SetItem("this", resultObject) };
        }

        // overwrite base with this fields
        resultObject = resultObject with { Fields = baseFields.SetItems(thisFields) };

        return this with { Objects = Objects.Add(type.TypeName, resultObject) };
    }

    private InterpreterState EvaluateAssignment(Assignment assignment)
    {
        var newVariables = Variables;
        var target = assignment.Target;

        if (target is VariableExp variable)
        {
            var newValue = EvaluateExpression(assignment.Value);
            newVariables = Variables.SetItem(variable.Name, newValue);
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

            if (Variables.GetValueOrDefault(objectName) is not Object obj)
            {
                throw new UnassignedVariableException(objectName, field.ObjectName);
            }

            if (!obj.Fields.ContainsKey(field.FieldName))
            {
                throw new NoSuchFieldException(field, obj.Type);
            }

            var oldObject = Variables[objectName] as Object ?? throw GetException();
            var newFieldValue = EvaluateExpression(assignment.Value);
            var newValue = oldObject with { Fields = oldObject.Fields.SetItem(field.FieldName, newFieldValue) };
            newVariables = Variables.SetItem(objectName, newValue);
        }

        return this with { Variables = newVariables };
    }

    public Value EvaluateList(ListExp list)
    {
        if (list.Elements.Count == 0)
        {
            return new EmptyList(list);
        }

        var values = list.Elements.Select(EvaluateExpression).ToImmutableList();
        var type = values.First().Type;

        return new TypedList(values, type);
    }

    public Value EvaluateExpression(Expression expression)
    {
        var value = expression switch
        {
            DiceExpression d => new SimpleDice(d.Count, d.Max),
            Natural n => new Integer(n.Value),
            DiceRoll dr => EvaluateRoll(dr),
            BooleanExpression be => new Boolean(be.Value),
            StringExpression se => new String(se.Value),
            BinaryOperation bo => EvaluateBinaryOperation(bo),
            FunctionInvocation fi => EvaluateFunctionInvocation(fi),
            ListExp le => EvaluateList(le),
            Parser.Grammar.Unit => new Unit(),
            UnaryMinus um => EvaluateUnaryMinus(um),
            IfExpression i => EvaluateIf(i),
            Block b => EvaluateBlock(b),
            ObjectCreation oc => EvaluateObjectCreation(oc),
            Reference r => EvaluateReference(r),
            _ => throw new NotImplementedException()
        };

        return value;
    }

    private Object EvaluateObjectCreation(ObjectCreation oc)
    {
        if (oc.Traits is null)
        {
            return Objects[oc.Type];
        }

        var anonymousType = new ObjectDeclaration(
            "anonymous" + (AnonymousObjectCount + 1),
            oc.Type,
            oc.Traits,
            new FieldList(new NodeList<FieldDeclaration>(Enumerable.Empty<FieldDeclaration>()), oc.End, oc.End),
            oc.Start,
            oc.End
        );

        var withAnonType =
            (this with { CurrentScope = AllScopes[anonymousType], AnonymousObjectCount = AnonymousObjectCount + 1 })
            .EvaluateObjectDeclaration(anonymousType);

        var creation = new ObjectCreation(
            anonymousType.Name,
            null,
            anonymousType.Start,
            anonymousType.End
        );
        return withAnonType.EvaluateObjectCreation(creation);
    }

    private Value EvaluateBlock(Block block)
    {
        Value? last = null;
        var blockInner = this;
        foreach (var inner in block.Inner)
        {
            if (inner is Expression e)
            {
                last = blockInner.EvaluateExpression(e);
            }
            else if (inner is Assignment a)
            {
                blockInner = blockInner.EvaluateAssignment(a);
            }
            else
            {
                throw new InvalidOperationException($"{inner} is not a valid statement inside a block.");
            }
        }

        return last ?? throw GetException();
    }

    private Value EvaluateIf(IfExpression ifExpression)
    {
        var condition = (Boolean)EvaluateExpression(ifExpression.Condition);

        if (condition.Value)
        {
            return EvaluateExpression(ifExpression.TrueValue);
        }

        return EvaluateExpression(ifExpression.FalseValue);
    }

    private Value EvaluateUnaryMinus(UnaryMinus um)
    {
        var inner = EvaluateExpression(um.Value);

        return inner switch
        {
            Integer i => new Integer(-i.Value),
            Dice d => new CompoundDice(() => -d.Roll()),
            _ => throw GetException()
        };
    }

    public Value EvaluateReference(Reference reference)
    {
        var value = reference switch
        {
            VariableExp v => Variables[v.Name],
            Base b => throw new ObjectSelfReferenceIsNotAllowed(b),
            This t => throw new ObjectSelfReferenceIsNotAllowed(t),
            FieldReference f => EvaluateFieldReference(f),
            _ => throw new ArgumentOutOfRangeException(nameof(reference), reference, "Unrecognized reference type.")
        };

        return value;
    }

    public Value EvaluateFieldReference(FieldReference reference)
    {
        var variableName = reference.ObjectName switch
        {
            Base => "base",
            This => "this",
            VariableExp v => v.Name
        };
        var variable = Variables[variableName] as Object ?? throw GetException();

        return variable.Fields[reference.FieldName];
    }

    public Integer EvaluateRoll(DiceRoll roll)
    {
        var innerResult = EvaluateExpression(roll.Dice);

        if (innerResult is Dice d)
        {
            return new Integer(d.Roll());
        }

        throw GetException();
    }

    public Value EvaluateBinaryOperation(BinaryOperation binaryOperation)
    {
        var left = EvaluateExpression(binaryOperation.Left);
        var right = EvaluateExpression(binaryOperation.Right);

        return binaryOperation switch
        {
            AdditionExp => EvaluateAddition(left, right),
            SubtractionExp => EvaluateSubtraction(left, right),
            MultiplicationExp => EvaluateMultiplication(left, right),
            DivisionExp => EvaluateDivision(left, right),
            BooleanAndExp => EvaluateBooleanAnd(left, right),
            BooleanOrExp => EvaluateBooleanOr(left, right),
            ConcatenationExp => EvaluateConcatenation(left, right),
            EqualExp => EvaluateEqual(left, right),
            NotEqualExp => EvaluateNotEqual(left, right),
            LessThanExp => EvaluateLessThan(left, right),
            GreaterThanExp => EvaluateGreaterThan(left, right),
            LessOrEqualThanExp => EvaluateLessOrEqual(left, right),
            GreaterOrEqualThanExp => EvaluateGreaterOrEqual(left, right),
            _ => throw new ArgumentOutOfRangeException(nameof(binaryOperation), binaryOperation,
                "Unrecognized binary operation type.")
        };
    }

    public Value EvaluateNumericOperation(Value left, Value right, Func<int, int, int> operation,
        [CallerMemberName] string operationName = "")
    {
        return (left, right) switch
        {
            (Integer n1, Integer n2) => new Integer(operation(n1.Value, n2.Value)),
            (Dice n1, Dice n2) => new CompoundDice(() => operation(n1.Roll(), n2.Roll())),
            (Integer n1, Dice n2) => new CompoundDice(() => operation(n1.Value, n2.Roll())),
            (Dice n1, Integer n2) => new CompoundDice(() => operation(n1.Roll(), n2.Value)),
            _ => throw GetException()
        };
    }

    public Value EvaluateAddition(Value left, Value right) =>
        EvaluateNumericOperation(left, right, (l, r) => l + r);

    public Value EvaluateSubtraction(Value left, Value right) =>
        EvaluateNumericOperation(left, right, (l, r) => l - r);

    public Value EvaluateMultiplication(Value left, Value right) =>
        EvaluateNumericOperation(left, right, (l, r) => l * r);

    public Value EvaluateDivision(Value left, Value right) =>
        EvaluateNumericOperation(left, right, (l, r) => l / r);

    public Value EvaluateBooleanAnd(Value left, Value right)
    {
        return (left, right) switch
        {
            (Boolean b1, Boolean b2) => new Boolean(b1.Value && b2.Value),
            _ => throw GetException()
        };
    }

    public Value EvaluateBooleanOr(Value left, Value right)
    {
        return (left, right) switch
        {
            (Boolean b1, Boolean b2) => new Boolean(b1.Value || b2.Value),
            _ => throw GetException()
        };
    }

    public Value EvaluateConcatenation(Value left, Value right)
    {
        return (left, right) switch
        {
            (String b1, String b2) => new String(b1.Value + b2.Value),
            (EmptyList, TypedList l) => l,
            (TypedList l, EmptyList) => l,
            (TypedList l1, TypedList l2) when l1.ElementType == l2.ElementType =>
                new TypedList(l1.Elements.AddRange(l2.Elements), l1.ElementType),
            _ => throw GetException()
        };
    }

    public Value EvaluateEqual(Value left, Value right)
    {
        return (left, right) switch
        {
            (Integer n1, Integer n2) => new Boolean(n1.Value == n2.Value),
            (String n1, String n2) => new Boolean(n1.Value == n2.Value),
            _ => throw GetException()
        };
    }

    public Value EvaluateNotEqual(Value left, Value right)
    {
        return (left, right) switch
        {
            (Integer n1, Integer n2) => new Boolean(n1.Value != n2.Value),
            (String n1, String n2) => new Boolean(n1.Value != n2.Value),
            _ => throw GetException()
        };
    }

    public Value EvaluateLessThan(Value left, Value right)
    {
        return (left, right) switch
        {
            (Integer n1, Integer n2) => new Boolean(n1.Value < n2.Value),
            _ => throw GetException()
        };
    }

    public Value EvaluateGreaterThan(Value left, Value right)
    {
        return (left, right) switch
        {
            (Integer n1, Integer n2) => new Boolean(n1.Value > n2.Value),
            _ => throw GetException()
        };
    }

    public Value EvaluateLessOrEqual(Value left, Value right)
    {
        return (left, right) switch
        {
            (Integer n1, Integer n2) => new Boolean(n1.Value <= n2.Value),
            _ => throw GetException()
        };
    }

    public Value EvaluateGreaterOrEqual(Value left, Value right)
    {
        return (left, right) switch
        {
            (Integer n1, Integer n2) => new Boolean(n1.Value >= n2.Value),
            _ => throw GetException()
        };
    }
}
