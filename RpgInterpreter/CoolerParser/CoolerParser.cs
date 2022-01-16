using System.Collections.Immutable;
using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.CoolerParser.ParsingFunctions;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser;

namespace RpgInterpreter.CoolerParser
{
    public class CoolerParser
    {
        public IEnumerable<Token> Source { get; }

        public CoolerParser(IEnumerable<Token> source) => Source = source;

        public Root Parse()
        {
            var parsingResult = new SourceState(ImmutableQueue.CreateRange(Source)).ParseProgram();

            if (!parsingResult.Source.Queue.IsEmpty)
            {
                throw new ParsingException("Unexpected tokens at the end of file.");
            }

            return parsingResult.Result;
        }
    }
}
