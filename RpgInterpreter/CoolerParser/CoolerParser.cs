using System.Collections.Immutable;
using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.CoolerParser.ParsingExceptions;
using RpgInterpreter.CoolerParser.ParsingFunctions;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser
{
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
}
