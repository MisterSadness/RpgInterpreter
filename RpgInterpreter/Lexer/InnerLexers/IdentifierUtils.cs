namespace RpgInterpreter.Lexer.InnerLexers;

public static class IdentifierUtils
{
    public static bool IsInnerIdentifier(char c) => char.IsLetterOrDigit(c) || c == '_';
}