using RpgInterpreter.Output;

namespace RpgInterpreter.ExceptionHandler;

public interface IExceptionHandler
{
    void RunAndHandle(Action action);
}

public abstract class ExceptionHandler : IExceptionHandler
{
    protected ExceptionHandler(IOutput output) => Output = output;
    public abstract void RunAndHandle(Action action);

    public IOutput Output { get; }
}