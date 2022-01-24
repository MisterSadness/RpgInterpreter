using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.TypeChecker.SemanticExceptions;

public class IncompatibleTypesException : SemanticException, IIntervalPositionedException
{
    public IncompatibleTypesException(Type expectedType, Type actualType, IPositioned positioned) :
        this(expectedType, actualType, positioned.Start, positioned.End) { }

    public IncompatibleTypesException(Type expectedType, Type actualType, Position start, Position end) :
        base($"Attempted to assign a value of type {actualType} to a variable of type {expectedType}.")
    {
        Start = start;
        End = end;
    }

    public Position Start { get; }
    public Position End { get; }
}