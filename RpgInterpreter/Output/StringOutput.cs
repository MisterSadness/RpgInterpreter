using System.Text;

namespace RpgInterpreter.Output;

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