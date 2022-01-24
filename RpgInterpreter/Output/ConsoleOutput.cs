namespace RpgInterpreter.Output;

public class ConsoleOutput : IOutput
{
    public void Write(string value)
    {
        Console.Write(value);
    }

    public void WriteLine(string value)
    {
        Console.WriteLine(value);
    }
}