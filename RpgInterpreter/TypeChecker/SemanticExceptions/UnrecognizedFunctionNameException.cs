using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.TypeChecker.SemanticExceptions;

public class UnrecognizedFunctionNameException : SemanticException, IIntervalPositionedException
{
    public UnrecognizedFunctionNameException(FunctionInvocation functionInvocation) :
        base($"There is no function called {functionInvocation.FunctionName}.")
    {
        Start = functionInvocation.Start;
        End = functionInvocation.End;
    }

    public Position Start { get; }
    public Position End { get; }
}