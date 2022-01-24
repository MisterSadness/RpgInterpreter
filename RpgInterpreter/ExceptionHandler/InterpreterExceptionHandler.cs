using RpgInterpreter.Output;
using RpgInterpreter.Runtime.SemanticExceptions;

namespace RpgInterpreter.ExceptionHandler;

public class InterpreterExceptionHandler : ExceptionHandler
{
    private readonly ErrorAreaPrinter _errorAreaPrinter;

    public InterpreterExceptionHandler(ErrorAreaPrinter errorAreaPrinter, IOutput output) : base(output) =>
        _errorAreaPrinter = errorAreaPrinter;

    public override void RunAndHandle(Action action)
    {
        try
        {
            action();
        }
        catch (SemanticException e)
        {
            Output.WriteLine($"Semantic exception occurred: {e.Message}");
            if (e is IPositionedException positioned)
            {
                Output.WriteLine(_errorAreaPrinter.FindErrorSurroundings(positioned));
            }
        }
    }
}