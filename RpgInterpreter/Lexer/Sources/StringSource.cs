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

    public char? Peek() => _position < _string.Length ? _string[_position] : null;

    public char? Pop()
    {
        if (_position >= _string.Length)
        {
            return null;
        }

        var result = _string[_position];
        _position++;
        return result;
    }
}