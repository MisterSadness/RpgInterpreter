using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;
using Type = RpgInterpreter.TypeChecker.Type;

namespace RpgInterpreter.Runtime.SemanticExceptions;

public class NonNumericNegationException : SemanticException, IIntervalPositionedException
{
    public NonNumericNegationException(UnaryMinus minus, Type type) : base(
        $"Cannot negate a value of type {type}")
    {
        Start = minus.Start;
        End = minus.End;
    }

    public Position Start { get; }
    public Position End { get; }
}