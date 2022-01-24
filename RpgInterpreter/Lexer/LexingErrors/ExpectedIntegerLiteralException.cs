namespace RpgInterpreter.Lexer.LexingErrors;

public class ExpectedIntegerLiteralException : LexingException
{
    public ExpectedIntegerLiteralException() : base("Encountered an invalid integer value.") { }
}