using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Runtime.SemanticExceptions;

public class UnrecognizedTypeException : SemanticException, IIntervalPositionedException
{
    public UnrecognizedTypeException(string typeName, IPositioned positioned) :
        base($"Identifier {typeName} does not correspond to any existing type.")
    {
        Start = positioned.Start;
        End = positioned.End;
    }

    public Position Start { get; }
    public Position End { get; }
}