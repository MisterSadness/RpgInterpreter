using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.TypeChecker.SemanticExceptions;

public class ObjectSelfReferenceIsNotAllowed : SemanticException, IIntervalPositionedException
{
    public ObjectSelfReferenceIsNotAllowed(IPositioned positioned) : base("Object self reference is not allowed.")
    {
        Start = positioned.Start;
        End = positioned.End;
    }

    public Position Start { get; }
    public Position End { get; }
}