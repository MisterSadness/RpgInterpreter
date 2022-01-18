namespace RpgInterpreter.ExceptionHandler;

public interface IPointPositionedException
{
    Position Position { get; }
}

public interface IIntervalPositionedException
{
    Position Start { get; }
    Position End { get; }
}