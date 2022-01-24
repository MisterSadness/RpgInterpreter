using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;
using Type = RpgInterpreter.TypeChecker.Type;

namespace RpgInterpreter.Runtime.SemanticExceptions;

public class NoSuchFieldException : SemanticException, IIntervalPositionedException
{
    public NoSuchFieldException(FieldReference fieldReference, Type type) :
        base($"Type {type} contains no definition of {fieldReference.FieldName}")
    {
        Start = fieldReference.Start;
        End = fieldReference.End;
    }

    public Position Start { get; }
    public Position End { get; }
}