namespace RpgInterpreter.Lexer.LexingErrors;

public class UnexpectedInputException : LexingException
{
    public UnexpectedInputException(char expected, char actual) : base(
        $"Expected character '{expected}' but encountered '{actual}'.") { }

    public UnexpectedInputException(char actual) : base(
        $"Character '{actual}' does not match the start of any token.") { }
}