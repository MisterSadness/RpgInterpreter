using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.TypeChecker.SemanticExceptions;

public class InvalidBinaryOperationException : SemanticException, IIntervalPositionedException
{
    public InvalidBinaryOperationException(Type l, Type r, string operation, IPositioned positioned) : base(
        $"There is no definition of {operation} for values of type {l} and {r}.")
    {
        Start = positioned.Start;
        End = positioned.End;
    }

    public Position Start { get; }
    public Position End { get; }
}