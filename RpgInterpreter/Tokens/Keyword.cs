namespace RpgInterpreter.Tokens
{
    public abstract record Keyword : Token;

    public record If : Keyword;

    public record Then : Keyword;

    public record Else : Keyword;

    public record Trait : Keyword;

    public record For : Keyword;

    public record With : Keyword;

    public record Extends : Keyword;

    public record And : Keyword;

    public record Base : Keyword;

    public record This : Keyword;
}