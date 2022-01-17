using System.Collections.Immutable;
using RpgInterpreter;
using RpgInterpreter.CoolerParser.ParsingFunctions;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreterTests.CoolerParserTests.Utils;

internal static class TokenEnumerableExtensions
{
    public static SourceState ToSourceState(this IEnumerable<Token> tokens) =>
        new(ImmutableQueue.CreateRange(
            tokens.Select(t => new PositionedToken(t, new Position(0, 0), new Position(0, 0)))));
}