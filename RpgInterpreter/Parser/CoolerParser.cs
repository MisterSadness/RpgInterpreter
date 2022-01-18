using System.Collections.Immutable;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreter.Parser.ParsingExceptions;
using RpgInterpreter.Parser.ParsingFunctions;

namespace RpgInterpreter.Parser;

public class CoolerParser
{
    public IEnumerable<PositionedToken> Source { get; }

    public CoolerParser(IEnumerable<PositionedToken> source) => Source = source;

    public Root Parse()
    {
        var parsingResult = new SourceState(ImmutableQueue.CreateRange(Source)).ParseProgram();

        var leftOver = parsingResult.Source.PeekPositionedOrDefault;
        if (leftOver is null)
        {
            throw new MissingMarkerException();
        }

        if (leftOver.Value is not LexingFinished)
        {
            throw new UnexpectedTokenException(leftOver);
        }

        return parsingResult.Result;
    }
}