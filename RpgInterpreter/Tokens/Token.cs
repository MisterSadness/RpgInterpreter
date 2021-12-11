namespace RpgInterpreter.Tokens;

public abstract record Token;

public record Whitespace : Token;

public record LowercaseIdentifier(string Identifier) : Token;

public record UppercaseIdentifier(string Identifier) : Token;

public record DiceLiteral(int Count, int Max) : Token;

public record NaturalLiteral(int Value) : Token;

public record BooleanLiteral(bool Value) : Token;

public record StringLiteral(string Value) : Token;