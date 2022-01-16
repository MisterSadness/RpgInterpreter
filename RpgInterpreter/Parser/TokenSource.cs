using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.Parser;

public class TokenSource
{
    private readonly IEnumerator<PositionedToken> _tokens;

    public TokenSource(IEnumerable<PositionedToken> tokens)
    {
        _tokens = tokens.GetEnumerator();
        _tokens.MoveNext();
    }

    public static TokenSource FromCharSource(ICharSource source) =>
        new(new TrackingRpgLexer().Tokenize(source));

    public PositionedToken Peek()
    {
        while (_tokens.Current.Value is Whitespace)
            _tokens.MoveNext();
        return _tokens.Current;
    }

    public PositionedToken Pop()
    {
        while (_tokens.Current.Value is Whitespace)
            _tokens.MoveNext();
        var current = _tokens.Current;
        _tokens.MoveNext();
        return current;
    }
}