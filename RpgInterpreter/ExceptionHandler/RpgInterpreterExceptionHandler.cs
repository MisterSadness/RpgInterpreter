using System.Collections.Immutable;

namespace RpgInterpreter.ExceptionHandler;

public class RpgInterpreterExceptionHandler : CompositeExceptionHandler
{
    public RpgInterpreterExceptionHandler(ErrorAreaPrinter errorAreaPrinter) : base(new IExceptionHandler[]
    {
        new InterpreterExceptionHandler(errorAreaPrinter),
        new ParserExceptionHandler(errorAreaPrinter),
        new LexerExceptionHandler(errorAreaPrinter)
    }.ToImmutableList()) { }
}