namespace RpgInterpreter.Runtime.RuntimeExceptions;

public class RuntimeException : Exception
{
    protected RuntimeException(string message) : base(message) { }
}