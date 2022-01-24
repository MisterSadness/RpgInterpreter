using System.Collections.Immutable;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreter.Parser.ParsingExceptions;
using RpgInterpreter.Parser.ParsingFunctions;

namespace RpgInterpreter.Parser;

public class CoolerParser
{
    public Root Parse(IEnumerable<PositionedToken> source)
    {
        var parsingResult = new SourceState(ImmutableQueue.CreateRange(source)).ParseProgram();

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