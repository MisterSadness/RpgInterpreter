using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Runtime.SemanticExceptions;

public class NotAnObjectException : SemanticException, IIntervalPositionedException
{
    public NotAnObjectException(string name, IPositioned positioned) : base($"Value {name} is not an object.")
    {
        Start = positioned.Start;
        End = positioned.End;
    }

    public Position Start { get; }
    public Position End { get; }
}