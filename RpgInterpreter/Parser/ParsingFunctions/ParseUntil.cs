using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.ParsingExceptions;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public ParseResult<IEnumerable<TElement>> ParseSeparated<TElement, TSeparator, TEnd>(
        Func<SourceState, IParseResult<TElement>> parseElement)
        where TSeparator : Token where TEnd : Token
    {
        var elements = new List<TElement>();
        var state = this;

        if (PeekOrDefault() is TEnd)
        {
            var parsedEnd = ParseToken<TEnd>();
            return new ParseResult<IEnumerable<TElement>>(parsedEnd.Source, elements);
        }

        while (true)
        {
            var element = parseElement(state);
            state = element.Source;
            elements.Add(element.Result);
            switch (state.PeekOrDefault())
            {
                case TSeparator:
                    var afterSeparator = state.ParseToken<TSeparator>();
                    state = afterSeparator.Source;
                    break;
                case TEnd:
                    var remaining = state.ParseToken<TEnd>().Source;
                    return new ParseResult<IEnumerable<TElement>>(remaining, elements);
                default:
                    throw new ExpectedTokenNotFoundException<TSeparator, TEnd>(state.PeekOrDefault(),
                        state.CurrentPosition);
            }
        }
    }

    public ParseResult<IEnumerable<TElement>> ParseTerminated<TElement, TSeparator, TEnd>(
        Func<SourceState, IParseResult<TElement>> parseElement)
        where TSeparator : Token where TEnd : Token
    {
        var elements = new List<TElement>();
        var state = this;

        while (state.PeekOrDefault() is not TEnd)
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