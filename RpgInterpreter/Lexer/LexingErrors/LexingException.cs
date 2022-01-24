namespace RpgInterpreter.Lexer.LexingErrors;

public abstract class LexingException : Exception
{
    protected LexingException(string message, Exception? inner = null) : base(message, inner) { }
}