using System.Collections.Immutable;
using RpgInterpreter.Output;

namespace RpgInterpreter.ExceptionHandler;

public class RpgInterpreterExceptionHandler : CompositeExceptionHandler
{
    public RpgInterpreterExceptionHandler(ErrorAreaPrinter errorAreaPrinter, IOutput output) : base(
        new IExceptionHandler[]
        {
            new InterpreterExceptionHandler(errorAreaPrinter, output),
            new ParserExceptionHandler(errorAreaPrinter, output),
            new LexerExceptionHandler(errorAreaPrinter, output)
        }.ToImmutableList()) { }
}