namespace RpgInterpreter.Lexer.Sources;

internal class TrackingCharSource : ICharSource
{
    private readonly ICharSource _inner;
    private int _column;

    private int _line;

    public TrackingCharSource(ICharSource inner) => _inner = inner;

    public Position Position => new(_line, _column);

    public char? Peek() => _inner.Peek();

    public char? Pop()
    {
        var c = _inner.Pop();

        if (c is '\n')
        {
            _line++;
            _column = 0;
        }
        else
        {
            _column++;
        }

        return c;
    }
}