using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.TypeChecker.SemanticExceptions;

public class TypeInferenceFailedException : SemanticException, IIntervalPositionedException
{
    public TypeInferenceFailedException(Type expectedType, Type actualType, IPositioned positioned) :
        this(expectedType, actualType, positioned.Start, positioned.End) { }

    public TypeInferenceFailedException(Type expectedType, Type actualType, Position start, Position end) :
        base($"Type inference failed: Expected {expectedType}, but got {actualType}.")
    {
        Start = start;
        End = end;
    }

    public TypeInferenceFailedException(IPositioned positioned) :
        base("Type inference failed: Could not deduce type of expression.")
    {
        Start = positioned.Start;
        End = positioned.End;
    }

    public Position Start { get; }
    public Position End { get; }
}