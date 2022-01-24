namespace RpgInterpreter.Output;

public class DoubleOutput : IOutput
{
    private readonly ConsoleOutput _consoleOutput = new();
    private readonly StringOutput _stringOutput = new();

    public void Write(string value)
    {
        _consoleOutput.Write(value);
        _stringOutput.Write(value);
    }

    public void WriteLine(string value)
    {
        _consoleOutput.WriteLine(value);
        _stringOutput.WriteLine(value);
    }

    public string Read() => _stringOutput.Read();
}