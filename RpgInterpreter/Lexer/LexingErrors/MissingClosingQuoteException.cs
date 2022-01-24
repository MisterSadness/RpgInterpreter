namespace RpgInterpreter.Lexer.LexingErrors;

public class MissingClosingQuoteException : LexingException
{
    public MissingClosingQuoteException() : base("String literal is missing an ending quote.") { }
}