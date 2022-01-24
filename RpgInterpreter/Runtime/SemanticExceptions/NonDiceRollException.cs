using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;
using Type = RpgInterpreter.TypeChecker.Type;

namespace RpgInterpreter.Runtime.SemanticExceptions;

internal class NonDiceRollException : SemanticException, IIntervalPositionedException
{
    public NonDiceRollException(IPositioned expression, Type actualType) :
        base(
            $"Cannot use the roll operator '()' on an object that is not of type {nameof(Dice)}, actual type {actualType}")
    {
        Start = expression.Start;
        End = expression.End;
    }

    public Position Start { get; }
    public Position End { get; }
}