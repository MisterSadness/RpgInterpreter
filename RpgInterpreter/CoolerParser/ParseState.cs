using System.Collections.Immutable;
using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Parser;
using RpgInterpreter.Tokens;
using RpgInterpreter.Utils;
using Assignment = RpgInterpreter.CoolerParser.Grammar.Assignment;
using Base = RpgInterpreter.CoolerParser.Grammar.Base;
using If = RpgInterpreter.CoolerParser.Grammar.If;
using This = RpgInterpreter.CoolerParser.Grammar.This;

namespace RpgInterpreter.CoolerParser;

public interface IParseResult<out T>
{
    T Result { get; }

    SourceState Source { get; }

    public IParseResult<TValue> WithValue<TValue>(TValue value);
}

public record ParseResult<T>(SourceState Source, T Result) : IParseResult<T>
{
    public IParseResult<TValue> WithValue<TValue>(TValue value) => new ParseResult<TValue>(Source, value);
}

public record SourceState(IImmutableQueue<Token> Queue)
{
    public virtual IParseResult<Root> ParseProgram()
    {
        var elementsState = ParseUntil<Statement, Semicolon, EndOfInput>(
            p => p.ParseStatement()
        );
        var elements = elementsState.Result;

        return elementsState.WithValue(new Root(elements));
    }

    public IParseResult<Statement> ParseStatement()
    {
        return Queue.PeekOrDefault() switch
        {
            UppercaseIdentifier => ParseObjectDeclaration(),
            Trait => ParseTraitDeclaration(),
            Set => ParseAssignment(),
            LowercaseIdentifier => ParseFunctionInvocationStatement(),
            Fun => ParseFunctionDeclaration(),
            _ => throw new ParsingException()
        };
    }

    public IParseResult<ObjectDeclaration> ParseObjectDeclaration()
    {
        var name = ParseToken<UppercaseIdentifier>();

        var sourceState = name.Source;
        UppercaseIdentifier? baseName = null;
        TraitList? traitList = null;

        if (sourceState.Queue.PeekOrDefault() is Extends)
        {
            var extends = name.Source.ParseToken<Extends>();

            var baseState = extends.Source.ParseToken<UppercaseIdentifier>();

            sourceState = baseState.Source;
            baseName = baseState.Result;
        }

        if (sourceState.Queue.PeekOrDefault() is With)
        {
            var traits = sourceState.ParseTraits();

            sourceState = traits.Source;
            traitList = traits.Result;
        }

        var fieldsState = sourceState.ParseFields();

        return fieldsState.WithValue(new ObjectDeclaration(
            name.Result,
            baseName,
            traitList,
            fieldsState.Result
        ));
    }

    public IParseResult<TraitDeclaration> ParseTraitDeclaration()
    {
        var keyword = ParseToken<Trait>();

        var name = keyword.Source.ParseToken<UppercaseIdentifier>();

        var state = name.Source;
        UppercaseIdentifier? baseName = null;

        if (state.Queue.PeekOrDefault() is For)
        {
            var forKeyword = name.Source.ParseToken<For>();

            var baseState = forKeyword.Source.ParseToken<UppercaseIdentifier>();

            state = baseState.Source;
            baseName = baseState.Result;
        }

        var fields = state.ParseFields();

        return fields.WithValue(new TraitDeclaration(name.Result, baseName, fields.Result));
    }

    public IParseResult<FieldList> ParseFields()
    {
        var open = ParseToken<OpenBrace>();

        var fields = open.Source.ParseUntil<FieldDeclaration, Comma, CloseBrace>(s => s.ParseFieldDeclaration());

        return fields.WithValue(new FieldList(fields.Result));
    }

    public IParseResult<FieldDeclaration> ParseFieldDeclaration()
    {
        var name = ParseToken<UppercaseIdentifier>();

        var colon = name.Source.ParseToken<Colon>();

        var definition = colon.Source.ParseExpression();

        return definition.WithValue(new FieldDeclaration(name.Result, definition.Result));
    }

    public IParseResult<Assignment> ParseAssignment()
    {
        var keywordState = ParseToken<Set>();

        var nameState = keywordState.Source.ParseToken<LowercaseIdentifier>();
        var name = nameState.Result;

        var equalSignState = nameState.Source.ParseToken<Tokens.Assignment>();

        var valueState = equalSignState.Source.ParseExpression();
        var value = valueState.Result;

        var assignment = new Assignment(name, value);
        return valueState.WithValue(assignment);
    }

    public IParseResult<Expression> ParseExpression()
    {
        IParseResult<Expression> parsedExpression = Queue.PeekOrDefault() switch
        {
            OpenBracket => ParseList(),
            NaturalLiteral => ParseNatural(),
            DiceLiteral => ParseDice(),
            BooleanLiteral => ParseBoolean(),
            StringLiteral => ParseString(),
            Minus => ParseUnaryMinus(),
            LowercaseIdentifier => ParseReference(),
            Tokens.Base or Tokens.This => ParseSelfReference(),
            OpenParen => ParseParentheses(),
            Tokens.If => ParseIf(),
            New => ParseObjectCreation(),
            OpenBrace => ParseBlock(),
            _ => throw new ParsingException()
        };

        if (parsedExpression.Source.Queue.PeekOrDefault() is OpenParen)
        {
            return parsedExpression.Source.ParseRoll(parsedExpression.Result);
        }

        if (parsedExpression.Source.Queue.PeekOrDefault() is Operator)
        {
            return parsedExpression.Source.ParseBinaryOperation(parsedExpression.Result);
        }
        return parsedExpression;
    }

    public IParseResult<Block> ParseBlock()
    {
        var open = ParseToken<OpenBrace>();

        var inner = open.Source.ParseUntil<IBlockInner, Semicolon, CloseBrace>(s => s.ParseBlockInner());
        var innerList = inner.Result.ToList();
        if (innerList.SkipLast(1).Any(x => x is not Assignment or If or FunctionInvocation))
        {
            throw new ParsingException();
        }

        var returnExpression = innerList.LastOrDefault(s => s is Expression) as Expression ?? new Unit();

        return inner.WithValue(new Block(innerList, returnExpression));
    }

    public IParseResult<IBlockInner> ParseBlockInner()
    {
        if (Queue.PeekOrDefault() is Set)
        {
            return ParseAssignment();
        }

        return ParseExpression();
    }

    public IParseResult<ObjectCreation> ParseObjectCreation()
    {
        var keyword = ParseToken<New>();

        var className = keyword.Source.ParseToken<UppercaseIdentifier>();

        var traits = className.Source.ParseTraits();

        return traits.WithValue(new ObjectCreation(className.Result, traits.Result));
    }

    public IParseResult<TraitList> ParseTraits()
    {
        var with = ParseToken<With>();

        var firstTrait = with.Source.ParseToken<UppercaseIdentifier>();

        var traitList = new List<UppercaseIdentifier> { firstTrait.Result };
        var state = firstTrait.Source;
        while (state.Queue.PeekOrDefault() is And)
        {
            var and = state.ParseToken<And>();

            var trait = and.Source.ParseToken<UppercaseIdentifier>();
            traitList.Add(trait.Result);
            state = trait.Source;
        }

        return new ParseResult<TraitList>(state, new TraitList(traitList));
    }

    public IParseResult<DiceRoll> ParseRoll(Expression expression)
    {
        var openParen = ParseToken<OpenParen>();
        var closeParen = openParen.Source.ParseToken<CloseParen>();

        return closeParen.WithValue(new DiceRoll(expression));
    }

    public IParseResult<BinaryOperation> ParseBinaryOperation(Expression left)
    {
        IParseResult<Operator> operatorState = Queue.PeekOrDefault() switch
        {
            Addition => ParseToken<Addition>(),
            Minus => ParseToken<Minus>(),
            Multiplication => ParseToken<Multiplication>(),
            Division => ParseToken<Division>(),
            BooleanAnd => ParseToken<BooleanAnd>(),
            BooleanOr => ParseToken<BooleanOr>(),
            Concatenation => ParseToken<Concatenation>(),
            Equality => ParseToken<Equality>(),
            Inequality => ParseToken<Inequality>(),
            Less => ParseToken<Less>(),
            Greater => ParseToken<Greater>(),
            LessOrEqual => ParseToken<LessOrEqual>(),
            GreaterOrEqual => ParseToken<GreaterOrEqual>(),
            _ => throw new ParsingException()
        };

        var rightState = operatorState.Source.ParseExpression();
        var right = rightState.Result;

        BinaryOperation binOp = operatorState.Result switch
        {
            Addition => new AdditionExp(left, right),
            Minus => new SubtractionExp(left, right),
            Multiplication => new MultiplicationExp(left, right),
            Division => new DivisionExp(left, right),
            BooleanAnd => new BooleanAndExp(left, right),
            BooleanOr => new BooleanOrExp(left, right),
            Concatenation => new ConcatenationExp(left, right),
            Equality => new EqualExp(left, right),
            Inequality => new NotEqualExp(left, right),
            Less => new LessThanExp(left, right),
            Greater => new GreaterThanExp(left, right),
            LessOrEqual => new LessOrEqualThanExp(left, right),
            GreaterOrEqual => new GreaterOrEqualThanExp(left, right),
            _ => throw new ParsingException()
        };

        return rightState.WithValue(binOp);
    }

    public IParseResult<If> ParseIf()
    {
        var ifState = ParseToken<Tokens.If>();

        var condition = ifState.Source.ParseExpression();

        var then = condition.Source.ParseToken<Then>();

        var trueValue = then.Source.ParseExpression();

        var elseState = trueValue.Source.ParseToken<Else>();

        var falseValue = elseState.Source.ParseExpression();

        return falseValue.WithValue(new If(
            condition.Result,
            trueValue.Result,
            falseValue.Result
        ));
    }

    public IParseResult<Parentheses> ParseParentheses()
    {
        var openParen = ParseToken<OpenParen>();

        var expression = openParen.Source.ParseExpression();

        var closeParen = expression.Source.ParseToken<CloseParen>();

        return closeParen.WithValue(new Parentheses(expression.Result));
    }

    public IParseResult<UnaryMinus> ParseUnaryMinus()
    {
        var minusState = ParseToken<Minus>();

        var expressionState = minusState.Source.ParseExpression();
        var expression = expressionState.Result;

        return expressionState.WithValue(new UnaryMinus(expression));
    }

    public IParseResult<Reference> ParseSelfReference()
    {
        IParseResult<NameReference> obj = Queue.PeekOrDefault() switch
        {
            Tokens.Base => ParseBase(),
            Tokens.This => ParseThis(),
            _ => throw new ParsingException()
        };

        if (obj.Source.Queue.PeekOrDefault() is Access)
        {
            var parsedAccess = obj.Source.ParseToken<Access>();
            var parsedField = parsedAccess.Source.ParseToken<UppercaseIdentifier>();

            return parsedField.WithValue(
                new FieldReference(obj.Result, parsedField.Result)
            );
        }

        return obj;
    }

    public IParseResult<Reference> ParseReference()
    {
        var parsedName = ParseToken<LowercaseIdentifier>();

        if (parsedName.Source.Queue.PeekOrDefault() is Access)
        {
            return parsedName.Source.ParseAccess(new Variable(parsedName.Result));
        }

        if (parsedName.Source.Queue.PeekOrDefault() is OpenParen)
        {
            var functionName = parsedName.Result;

            var parsedOpenParen = parsedName.Source.ParseToken<OpenParen>();

            var arguments = parsedOpenParen.Source.ParseUntil<Expression, Comma, CloseParen>(s => s.ParseExpression());

            return arguments.WithValue(new FunctionInvocation(functionName, arguments.Result));
        }

        return parsedName.WithValue(new Variable(parsedName.Result));
    }

    public IParseResult<FieldReference> ParseAccess(NameReference objectName)
    {
        var parsedAccess = ParseToken<Access>();
        var parsedField = parsedAccess.Source.ParseToken<UppercaseIdentifier>();

        return parsedField.WithValue(
            new FieldReference(objectName, parsedField.Result)
        );
    }

    public IParseResult<List> ParseList()
    {
        var openBracketState = ParseToken<OpenBracket>();

        var elementsState = openBracketState.Source.ParseUntil<Expression, Comma, CloseBracket>(
            p => p.ParseExpression()
        );
        var elements = elementsState.Result;

        return elementsState.WithValue(new List(elements));
    }

    public ParseResult<IEnumerable<TElement>> ParseUntil<TElement, TSeparator, TEnd>(
        Func<SourceState, IParseResult<TElement>> parseElement)
        where TSeparator : Token where TEnd : Token
    {
        var elements = new List<TElement>();
        var state = this;

        while (true)
        {
            var element = parseElement(state);
            state = element.Source;
            elements.Add(element.Result);
            switch (state.Queue.PeekOrDefault())
            {
                case TSeparator:
                    var afterSeparator = state.ParseToken<TSeparator>();
                    state = afterSeparator.Source;
                    break;
                case TEnd:
                    var remaining = state.ParseToken<TEnd>().Source;
                    return new ParseResult<IEnumerable<TElement>>(remaining, elements);
                default:
                    throw new ParsingException();
            }
        }
    }

    public IParseResult<Natural> ParseNatural()
    {
        var parsed = ParseToken<NaturalLiteral>();

        return parsed.WithValue(new Natural(parsed.Result));
    }

    public IParseResult<Dice> ParseDice()
    {
        var parsed = ParseToken<DiceLiteral>();

        return parsed.WithValue(new Dice(parsed.Result));
    }

    public IParseResult<BooleanExpression> ParseBoolean()
    {
        var parsed = ParseToken<BooleanLiteral>();

        return parsed.WithValue(new BooleanExpression(parsed.Result));
    }

    public IParseResult<StringExpression> ParseString()
    {
        var parsed = ParseToken<StringLiteral>();

        return parsed.WithValue(new StringExpression(parsed.Result));
    }
    public IParseResult<Base> ParseBase()
    {
        var parsed = ParseToken<Tokens.This>();

        return parsed.WithValue(new Base());
    }
    public IParseResult<This> ParseThis()
    {
        var parsed = ParseToken<Tokens.This>();

        return parsed.WithValue(new This());
    }
    public IParseResult<FunctionInvocation> ParseFunctionInvocation()
    {
        var parsedName = ParseToken<LowercaseIdentifier>();
        var functionName = parsedName.Result;

        var parsedOpenParen = parsedName.Source.ParseToken<OpenParen>();

        var arguments = parsedOpenParen.Source.ParseUntil<Expression, Comma, CloseParen>(s => s.ParseExpression());

        return arguments.WithValue(new FunctionInvocation(functionName, arguments.Result));
    }

    public IParseResult<FunctionInvocationStatement> ParseFunctionInvocationStatement()
    {
        var parsed = ParseFunctionInvocation();
        return parsed.WithValue(new FunctionInvocationStatement(parsed.Result));
    }

    public IParseResult<FunctionDeclaration> ParseFunctionDeclaration() => null!;

    public IParseResult<TToken> ParseToken<TToken>() where TToken : Token
    {
        var topToken = Queue.FirstOrDefault();

        if (topToken is TToken token)
        {
            return new ParseResult<TToken>(Queue.Dequeue().ToState(), token);
        }

        throw new ParsingException();
    }
}