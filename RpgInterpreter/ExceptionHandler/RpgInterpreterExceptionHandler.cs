using System.Collections.Immutable;

namespace RpgInterpreter.ExceptionHandler;

public class RpgInterpreterExceptionHandler : CompositeExceptionHandler
{
    public RpgInterpreterExceptionHandler() : base(new IExceptionHandler[]
    {
        new InterpreterExceptionHandler(),
        new ParserExceptionHandler(),
        new LexerExceptionHandler()
    }.ToImmutableList()) { }
}