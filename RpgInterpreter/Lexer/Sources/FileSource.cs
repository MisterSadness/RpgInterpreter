using Optional;
using RpgInterpreter.Utils;

namespace RpgInterpreter.Lexer.Sources;

public class FileSource : ICharSource, IDisposable
{
    private readonly StreamReader _reader;

    public FileSource(string path) => _reader = new StreamReader(path);

    public virtual Option<char> Peek() => ToOptional(_reader.Peek());

    public virtual Option<char> Pop() => ToOptional(_reader.Read());

    public void Dispose()
    {
        _reader.Dispose();
    }

    private static Option<char> ToOptional(int i) => i.SomeWhen(i != -1).Map(c => (char)c);
}