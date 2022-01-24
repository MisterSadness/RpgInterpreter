using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.TypeChecker.SemanticExceptions;

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