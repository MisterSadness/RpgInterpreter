using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Runtime.SemanticExceptions;

public class FunctionRedeclarationException : SemanticException, IIntervalPositionedException
{
    public FunctionRedeclarationException(FunctionDeclaration functionDeclaration) :
        base($"Function {functionDeclaration} already exists.")
    {
        Start = functionDeclaration.Start;
        End = functionDeclaration.Body.Start;
    }

    public Position Start { get; }
    public Position End { get; }
}