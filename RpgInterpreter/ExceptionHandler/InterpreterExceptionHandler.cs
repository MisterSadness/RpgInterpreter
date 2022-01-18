using RpgInterpreter.Runtime;

namespace RpgInterpreter.ExceptionHandler;

public class InterpreterExceptionHandler : IExceptionHandler
{
    public void RunAndHandle(Action action)
    {
        try
        {
            action();
        }
        catch (SemanticException e)
        {
            Console.WriteLine("Semantic exception occurred:");
            Console.WriteLine(e);
        }
    }
}