using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Runtime.SemanticExceptions;

public class UnassignedVariableException : SemanticException, IIntervalPositionedException
{
    public UnassignedVariableException(VariableExp v) : this(v.Name, v) { }

    public UnassignedVariableException(string name, IPositioned v) : base(
        $"There is no value corresponding to name {name} in current scope.")
    {
        Start = v.Start;
        End = v.End;
    }

    public Position Start { get; }
    public Position End { get; }
}