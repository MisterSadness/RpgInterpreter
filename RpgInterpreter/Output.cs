using System.Text;

namespace RpgInterpreter;

public interface IOutput
{
    public void Write(string value);
    public void WriteLine(string value);
}

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

public class StringOutput : IOutput
{
    private readonly StringBuilder _stringBuilder = new();
    public void Write(string value) => _stringBuilder.Append(value);

    public void WriteLine(string value)
    {
        _stringBuilder.AppendLine(value);
    }

    public string Read() => _stringBuilder.ToString();
}

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