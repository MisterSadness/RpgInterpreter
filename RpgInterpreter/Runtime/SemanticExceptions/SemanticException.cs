namespace RpgInterpreter.Runtime.SemanticExceptions;

public class SemanticException : Exception
{
    public SemanticException(string message) : base(message) { }
}