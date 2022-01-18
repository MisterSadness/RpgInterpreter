using RpgInterpreter.Runtime;

namespace RpgInterpreter.ExceptionHandler;

public class InterpreterExceptionHandler : IExceptionHandler
{
    private readonly ErrorAreaPrinter _errorAreaPrinter;

    public InterpreterExceptionHandler(ErrorAreaPrinter errorAreaPrinter) => _errorAreaPrinter = errorAreaPrinter;

    public void RunAndHandle(Action action)
    {
        try
        {
            action();
        }
        catch (SemanticException e)
        {
            Console.WriteLine("Semantic exception occurred:");
            if (e is IPointPositionedException positioned)
            {
                Console.WriteLine(_errorAreaPrinter.FindErrorSurroundings(positioned));
            }

            Console.WriteLine(e);
        }
    }
}