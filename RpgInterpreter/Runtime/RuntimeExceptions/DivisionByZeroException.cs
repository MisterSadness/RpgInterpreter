using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Runtime.RuntimeExceptions;

public class DivisionByZeroException : RuntimeException, IIntervalPositionedException
{
    public DivisionByZeroException(IPositioned positioned) : base("Attempted to divide by zero.")
    {
        Start = positioned.Start;
        End = positioned.End;
    }

    public Position Start { get; }
    public Position End { get; }
}