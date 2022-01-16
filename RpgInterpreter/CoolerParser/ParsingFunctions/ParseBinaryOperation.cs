using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser;
using RpgInterpreter.Utils;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    private Precedence GetPrecedence(Operator op)
    {
        return op switch
        {
            Multiplication or Division => Precedence.Multiplication,
            Concatenation => Precedence.Concatenation,
            Addition or Minus => Precedence.Addition,
            Equality or Inequality or Less or Greater or LessOrEqual or GreaterOrEqual => Precedence.Comparison,
            BooleanAnd or BooleanOr => Precedence.Boolean,
            _ => throw new InvalidOperationException()
        };
    }

    public IParseResult<BinaryOperation>? ParseBinaryOperation(Expression left, Precedence currentPrecedence)
    {
        // 2 * 2 - 4
        // prec(*) > prec(-)
        // stop after 2 * 2
        // * 4

        // 2 - 2 * 4
        // prec(-) <= prec(*)
        // continue
        IParseResult<Operator>? operatorState = Queue.PeekOrDefault() switch
        {
            Multiplication => ParseToken<Multiplication>(),
            Division => ParseToken<Division>(),

            Concatenation when currentPrecedence <= Precedence.Concatenation => ParseToken<Concatenation>(),

            Addition when currentPrecedence <= Precedence.Addition => ParseToken<Addition>(),
            Minus when currentPrecedence <= Precedence.Addition => ParseToken<Minus>(),

            Equality when currentPrecedence <= Precedence.Comparison => ParseToken<Equality>(),
            Inequality when currentPrecedence <= Precedence.Comparison => ParseToken<Inequality>(),
            Less when currentPrecedence <= Precedence.Comparison => ParseToken<Less>(),
            Greater when currentPrecedence <= Precedence.Comparison => ParseToken<Greater>(),
            LessOrEqual when currentPrecedence <= Precedence.Comparison => ParseToken<LessOrEqual>(),
            GreaterOrEqual when currentPrecedence <= Precedence.Comparison => ParseToken<GreaterOrEqual>(),

            BooleanAnd when currentPrecedence <= Precedence.Boolean => ParseToken<BooleanAnd>(),
            BooleanOr when currentPrecedence <= Precedence.Boolean => ParseToken<BooleanOr>(),
            _ => null
        };

        if (operatorState is null)
        {
            return null;
        }

        var newPrecedence = GetPrecedence(operatorState.Result);

        var rightState = operatorState.Source.ParseExpression(newPrecedence);
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
            _ => throw new ParsingException($"Invalid operator {operatorState.Result.GetType().Name}")
        };

        return rightState.WithValue(binOp);
    }
}