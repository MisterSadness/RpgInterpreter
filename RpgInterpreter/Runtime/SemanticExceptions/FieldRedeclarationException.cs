using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Runtime.SemanticExceptions;

public class FieldRedeclarationException : SemanticException, IIntervalPositionedException
{
    public FieldRedeclarationException(IPositioned positioned, string type, string fieldName) :
        base($"Type {type} already contains a definition of {fieldName}")
    {
        Start = positioned.Start;
        End = positioned.End;
    }

    public FieldRedeclarationException(FieldDeclaration fieldDeclaration, string type) :
        this(fieldDeclaration, type, fieldDeclaration.Name) { }

    public Position Start { get; }
    public Position End { get; }
}