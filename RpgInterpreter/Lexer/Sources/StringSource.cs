using Optional;
using RpgInterpreter.Utils;

namespace RpgInterpreter.Lexer.Sources;

public class StringSource : ICharSource
{
    private readonly string _string;
    private int _position;

    public StringSource(string s)
    {
        _string = s;
        _position = 0;
    }

    public Option<char> Peek() => _position
        .SomeWhen(_position < _string.Length)
        .Map(p => _string[p]);

    public Option<char> Pop()
    {
        var result = Peek();
        result.MatchSome(_ => _position++);
        return result;
    }
}