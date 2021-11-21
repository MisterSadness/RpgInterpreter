namespace RpgInterpreter.Tokens
{
    public abstract record Token;

    public record Whitespace : Token;

    public record LowercaseIdentifier(string Identifier) : Token;

    public record UppercaseIdentifier(string Identifier) : Token;

    public record Dice(int Count, int Max) : Token;

    public record BooleanLiteral(bool Value) : Token;

    public record StringLiteral(string Value) : Token;
}
