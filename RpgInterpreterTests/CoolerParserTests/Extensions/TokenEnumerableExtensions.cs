using System.Collections.Immutable;
using RpgInterpreter.CoolerParser.ParsingFunctions;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreterTests.CoolerParserTests.Extensions;

internal static class TokenEnumerableExtensions
{
    public static SourceState ToSourceState(this IEnumerable<Token> tokens) =>
        new SourceState(ImmutableQueue.CreateRange(tokens));
}