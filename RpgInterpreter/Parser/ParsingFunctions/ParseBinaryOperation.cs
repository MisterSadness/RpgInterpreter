using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreter.Parser.ParsingExceptions;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
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
        var start = left.Start;
        // 2 * 2 - 4
        // prec(*) > prec(-)
        // stop after 2 * 2
        // * 4

        // 2 - 2 * 4
        // prec(-) <= prec(*)
        // continue
        IParseResult<Operator>? operatorState = PeekOrDefault() switch
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
        var end = rightState.Source.CurrentPosition;

        BinaryOperation binOp = operatorState.Result switch
        {
            Addition => new AdditionExp(left, right, start, end),
            Minus => new SubtractionExp(left, right, start, end),
            Multiplication => new MultiplicationExp(left, right, start, end),
            Division => new DivisionExp(left, right, start, end),
            BooleanAnd => new BooleanAndExp(left, right, start, end),
            BooleanOr => new BooleanOrExp(left, right, start, end),
            Concatenation => new ConcatenationExp(left, right, start, end),
            Equality => new EqualExp(left, right, start, end),
            Inequality => new NotEqualExp(left, right, start, end),
            Less => new LessThanExp(left, right, start, end),
            Greater => new GreaterThanExp(left, right, start, end),
            LessOrEqual => new LessOrEqualThanExp(left, right, start, end),
            GreaterOrEqual => new GreaterOrEqualThanExp(left, right, start, end),
            _ => throw new ParsingException($"Invalid operator {operatorState.Result.GetType().Name}")
        };

        return rightState.WithValue(binOp);
    }
}