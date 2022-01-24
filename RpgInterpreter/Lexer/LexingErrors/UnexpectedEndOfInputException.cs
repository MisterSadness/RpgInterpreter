namespace RpgInterpreter.Lexer.LexingErrors;

public class UnexpectedEndOfInputException : LexingException
{
    public UnexpectedEndOfInputException() : base("Encountered an unexpected end of input.") { }
}