using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;
using Type = RpgInterpreter.TypeChecker.Type;

namespace RpgInterpreter.Runtime.SemanticExceptions;

public class ConflictingTraitsDeclaration : SemanticException, IIntervalPositionedException
{
    public ConflictingTraitsDeclaration(IPositioned positioned, string type, Type fieldType, string fieldName,
        string conflictingTrait,
        Type conflictingType) :
        base($"Type {type} already contains a definition of {fieldName}: {fieldType}, " +
             $"trait {conflictingTrait} tried to redefine it as type {conflictingType}.")
    {
        Start = positioned.Start;
        End = positioned.End;
    }

    public Position Start { get; }
    public Position End { get; }
}