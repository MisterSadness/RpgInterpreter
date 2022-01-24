using RpgInterpreter.Output;
using RpgInterpreter.Runtime.RuntimeExceptions;

namespace RpgInterpreter.ExceptionHandler;

public class RuntimeExceptionHandler : ExceptionHandler
{
    private readonly ErrorAreaPrinter _errorAreaPrinter;

    public RuntimeExceptionHandler(ErrorAreaPrinter errorAreaPrinter, IOutput output) : base(output) =>
        _errorAreaPrinter = errorAreaPrinter;

    public override void RunAndHandle(Action action)
    {
        try
        {
            action();
        }
        catch (RuntimeException e)
        {
            Output.WriteLine($"Runtime exception occurred: {e.Message}");
            if (e is IPositionedException positioned)
            {
                Output.WriteLine(_errorAreaPrinter.FindErrorSurroundings(positioned));
            }
        }
    }
}