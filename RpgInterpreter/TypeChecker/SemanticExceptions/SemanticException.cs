namespace RpgInterpreter.TypeChecker.SemanticExceptions;

public class SemanticException : Exception
{
    public SemanticException(string message) : base(message) { }
}