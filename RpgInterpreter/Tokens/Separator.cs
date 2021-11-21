using RpgInterpreter.Lexer;

namespace RpgInterpreter.Tokens
{
    public abstract record Separator : Token;
    public record OpenParen : Separator;
    public record CloseParen : Separator;
    public record OpenBracket : Separator;
    public record CloseBracket : Separator;
    public record OpenBrace : Separator;
    public record CloseBrace : Separator;
    public record Colon : Separator;
}