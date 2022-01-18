using RpgInterpreter.Lexer.LexingErrors;

namespace RpgInterpreter.ExceptionHandler;

public class LexerExceptionHandler : IExceptionHandler
{
    private readonly ErrorAreaPrinter _errorAreaPrinter;

    public LexerExceptionHandler(ErrorAreaPrinter errorAreaPrinter) => _errorAreaPrinter = errorAreaPrinter;

    public void RunAndHandle(Action action)
    {
        try
        {
            action();
        }
        catch (LexingException e)
        {
            Console.WriteLine("Lexing exception occurred:");
            if (e is IPointPositionedException positioned)
            {
                Console.WriteLine(_errorAreaPrinter.FindErrorSurroundings(positioned));
            }

            Console.WriteLine(e);
        }
    }
}