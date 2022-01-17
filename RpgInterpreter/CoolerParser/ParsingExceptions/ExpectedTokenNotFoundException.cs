using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingExceptions;

public class ExpectedTokenNotFoundException<TExpected> : ParsingException
{
    public ExpectedTokenNotFoundException(Token? actual, Position position) : base(
        $"Expected to find {typeof(TExpected).Name} but found {actual?.ToString() ?? "nothing"} at position {position.Formatted}.") { }
}

public class ExpectedTokenNotFoundException<TExpected1, TExpected2> : ParsingException
{
    public ExpectedTokenNotFoundException(Token? actual, Position position) : base(
        $"Expected to find {typeof(TExpected1).Name} or {typeof(TExpected2).Name} but found {actual?.ToString() ?? "nothing"} at position {position.Formatted}.") { }
}