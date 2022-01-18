using RpgInterpreter.Lexer.LexingErrors;

namespace RpgInterpreter.ExceptionHandler;

public class LexerExceptionHandler : IExceptionHandler
{
    public void RunAndHandle(Action action)
    {
        try
        {
            action();
        }
        catch (LexingException e)
        {
            Console.WriteLine("Lexing exception occurred:");
            Console.WriteLine(e);
        }
    }
}