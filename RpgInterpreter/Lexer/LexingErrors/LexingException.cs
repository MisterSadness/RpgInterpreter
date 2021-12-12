namespace RpgInterpreter.Lexer.LexingErrors;

public abstract class LexingException : Exception
{
    protected LexingException(string? message = null, Exception? inner = null) : base(message, inner) { }
}