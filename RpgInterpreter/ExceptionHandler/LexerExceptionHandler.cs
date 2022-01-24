using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Output;

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
        catch (PositionedLexingException e)
        {
            Output.WriteLine($"Lexing exception occurred: {e.Inner.Message}");
            if (e is IPositionedException positioned)
            {
                Output.WriteLine(_errorAreaPrinter.FindErrorSurroundings(positioned));
            }
        }
    }
}