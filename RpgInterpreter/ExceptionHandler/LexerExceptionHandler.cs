using RpgInterpreter.Lexer.LexingErrors;

namespace RpgInterpreter.ExceptionHandler;

public class LexerExceptionHandler : ExceptionHandler
{
    private readonly ErrorAreaPrinter _errorAreaPrinter;

    public LexerExceptionHandler(ErrorAreaPrinter errorAreaPrinter, IOutput output) : base(output) =>
        _errorAreaPrinter = errorAreaPrinter;

    public override void RunAndHandle(Action action)
    {
        try
        {
            action();
        }
        catch (LexingException e)
        {
            Output.WriteLine($"Lexing exception occurred: {e.Message}");
            if (e is IPositionedException positioned)
            {
                Output.WriteLine(_errorAreaPrinter.FindErrorSurroundings(positioned));
            }
        }
    }
}