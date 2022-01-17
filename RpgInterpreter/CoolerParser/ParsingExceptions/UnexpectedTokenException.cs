using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingExceptions;

public class UnexpectedTokenException : ParsingException
{
    public UnexpectedTokenException(PositionedToken unexpected) : base(
        $"An unexpected token {unexpected} found at position {unexpected.Start.Formatted}.") { }
}