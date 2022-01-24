namespace RpgInterpreter.Lexer.LexingErrors;

public class InvalidCharacterException : LexingException
{
    public InvalidCharacterException(char invalid) : base($"Character '{invalid}' is not a valid String element.") { }
}