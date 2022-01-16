using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser;
using RpgInterpreter.Utils;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public ParseResult<IEnumerable<TElement>> ParseSeparated<TElement, TSeparator, TEnd>(
        Func<SourceState, IParseResult<TElement>> parseElement)
        where TSeparator : Token where TEnd : Token
    {
        var elements = new List<TElement>();
        var state = this;

        while (true)
        {
            var element = parseElement(state);
            state = element.Source;
            elements.Add(element.Result);
            switch (state.Queue.PeekOrDefault())
            {
                case TSeparator:
                    var afterSeparator = state.ParseToken<TSeparator>();
                    state = afterSeparator.Source;
                    break;
                case TEnd:
                    var remaining = state.ParseToken<TEnd>().Source;
                    return new ParseResult<IEnumerable<TElement>>(remaining, elements);
                default:
                    throw new ParsingException(
                        $"Expected {typeof(TSeparator).Name} separator or {typeof(TEnd).Name} terminator.");
            }
        }
    }

    public ParseResult<IEnumerable<TElement>> ParseTerminated<TElement, TSeparator, TEnd>(
        Func<SourceState, IParseResult<TElement>> parseElement)
        where TSeparator : Token where TEnd : Token
    {
        var elements = new List<TElement>();
        var state = this;

        while (state.Queue.PeekOrDefault() is not TEnd)
        {
            var element = parseElement(state);
            state = element.Source;
            elements.Add(element.Result);
            var afterSeparator = state.ParseToken<TSeparator>();
            state = afterSeparator.Source;
        }

        var remaining = state.ParseToken<TEnd>().Source;
        return new ParseResult<IEnumerable<TElement>>(remaining, elements);
    }
}