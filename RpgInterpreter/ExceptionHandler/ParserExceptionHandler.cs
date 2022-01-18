using RpgInterpreter.Parser.ParsingExceptions;

namespace RpgInterpreter.ExceptionHandler;

public class ParserExceptionHandler : IExceptionHandler
{
    public void RunAndHandle(Action action)
    {
        try
        {
            action();
        }
        catch (ParsingException e)
        {
            Console.WriteLine("Parsing exception occurred:");
            Console.WriteLine(e);
        }
    }
}