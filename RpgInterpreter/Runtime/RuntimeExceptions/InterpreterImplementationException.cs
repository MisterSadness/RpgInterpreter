namespace RpgInterpreter.Runtime.RuntimeExceptions;

public class InterpreterImplementationException : RuntimeException
{
    public InterpreterImplementationException() : base(
        "RPG Interpreter doesn't correctly handle this case.") { }
}