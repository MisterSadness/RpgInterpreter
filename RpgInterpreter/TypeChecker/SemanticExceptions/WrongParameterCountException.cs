using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.TypeChecker.SemanticExceptions;

public class WrongParameterCountException : SemanticException, IIntervalPositionedException
{
    public WrongParameterCountException(FunctionInvocation functionInvocation, int expectedCount) :
        base($"Function {functionInvocation.FunctionName} takes {expectedCount} parameters, " +
             $"but was given {functionInvocation.Arguments.Count()}.")
    {
        Start = functionInvocation.Start;
        End = functionInvocation.End;
    }

    public Position Start { get; }
    public Position End { get; }
}