namespace RpgInterpreter.ExceptionHandler;

public interface IExceptionHandler
{
    void RunAndHandle(Action action);
}