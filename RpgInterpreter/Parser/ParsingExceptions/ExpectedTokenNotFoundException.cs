using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.Parser.ParsingExceptions;

public class ExpectedTokenNotFoundException<TExpected> : ParsingException, IPointPositionedException
{
    public ExpectedTokenNotFoundException(Token? actual, Position position) : base(
        $"Expected to find {typeof(TExpected).Name} but found {actual?.ToString() ?? "nothing"} at position {position.Formatted}.") =>
        Position = position;

    public Position Position { get; }
}

public class ExpectedTokenNotFoundException<TExpected1, TExpected2> : ParsingException, IPointPositionedException
{
    public Position Position { get; }

    public ExpectedTokenNotFoundException(Token? actual, Position position) : base(
        $"Expected to find {typeof(TExpected1).Name} or {typeof(TExpected2).Name} but found {actual?.ToString() ?? "nothing"} at position {position.Formatted}.") =>
        Position = position;
}