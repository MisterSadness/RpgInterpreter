using RpgInterpreter.Output;
using RpgInterpreter.Parser.ParsingExceptions;

namespace RpgInterpreter.ExceptionHandler;

public class ParserExceptionHandler : ExceptionHandler
{
    private readonly ErrorAreaPrinter _errorAreaPrinter;

    public ParserExceptionHandler(ErrorAreaPrinter errorAreaPrinter, IOutput output) : base(output) =>
        _errorAreaPrinter = errorAreaPrinter;

    public override void RunAndHandle(Action action)
    {
        try
        {
            action();
        }
        catch (ParsingException e)
        {
            Output.WriteLine($"Parsing exception occurred: {e.Message}");
            if (e is IPositionedException positioned)
            {
                Output.WriteLine(_errorAreaPrinter.FindErrorSurroundings(positioned));
            }
        }
    }
}