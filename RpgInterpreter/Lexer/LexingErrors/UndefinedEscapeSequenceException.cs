namespace RpgInterpreter.Lexer.LexingErrors;

public class UndefinedEscapeSequenceException : LexingException
{
    public UndefinedEscapeSequenceException(char c) : base($"Sequence '\\{c}' is not a recognized escape sequence.") { }
}