using RpgInterpreter.Parser.ParsingExceptions;

namespace RpgInterpreter.ExceptionHandler;

public class ParserExceptionHandler : IExceptionHandler
{
    private readonly ErrorAreaPrinter _errorAreaPrinter;

    public ParserExceptionHandler(ErrorAreaPrinter errorAreaPrinter) => _errorAreaPrinter = errorAreaPrinter;

    public void RunAndHandle(Action action)
    {
        try
        {
            action();
        }
        catch (ParsingException e)
        {
            Console.WriteLine("Parsing exception occurred:");
            if (e is IPointPositionedException positioned)
            {
                Console.WriteLine(_errorAreaPrinter.FindErrorSurroundings(positioned));
            }

            Console.WriteLine(e);
        }
    }
}