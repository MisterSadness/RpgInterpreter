using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.Parser.ParsingExceptions;

public class UnexpectedTokenException : ParsingException, IPointPositionedException
{
    public UnexpectedTokenException(PositionedToken unexpected) : base(
        $"An unexpected token {unexpected} found at position {unexpected.Start.Formatted}.") =>
        Position = unexpected.Start;

    public Position Position { get; }
}