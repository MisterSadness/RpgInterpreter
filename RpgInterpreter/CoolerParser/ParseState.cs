using RpgInterpreter.CoolerParser.ParsingFunctions;

namespace RpgInterpreter.CoolerParser;

public interface IParseResult<out T>
{
    T Result { get; }

    SourceState Source { get; }

    public IParseResult<TValue> WithValue<TValue>(TValue value);
}

public record ParseResult<T>(SourceState Source, T Result) : IParseResult<T>
{
    public IParseResult<TValue> WithValue<TValue>(TValue value) => new ParseResult<TValue>(Source, value);
}