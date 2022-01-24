namespace RpgInterpreter.ExceptionHandler;

public interface IPositionedException { }

public interface IPointPositionedException : IPositionedException
{
    Position Position { get; }
}

public interface IIntervalPositionedException : IPositionedException
{
    Position Start { get; }
    Position End { get; }
}