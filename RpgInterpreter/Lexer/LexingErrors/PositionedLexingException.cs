using RpgInterpreter.Lexer.Sources;

namespace RpgInterpreter.Lexer.LexingErrors;

public class PositionedLexingException : LexingException
{
    public PositionedLexingException(LexingException inner, Position position) : base(
        $"Lexing failed at: {position.Formatted}",
        inner)
    {
        Inner = inner;
        Position = position;
    }

    public LexingException Inner { get; }
    public Position Position { get; }
}