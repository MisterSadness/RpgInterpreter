using RpgInterpreter.Tokens;

namespace RpgInterpreter.Parser;

public abstract record Terminal : Symbol
{
    public abstract Type TokenType { get; }
}

public record Epsilon : Terminal<Whitespace>;

public record Terminal<T> : Terminal where T : Token
{
    public override Type TokenType { get; } = typeof(T);
}
