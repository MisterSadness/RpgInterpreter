using System.Collections.Immutable;
using RpgInterpreter;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.ParsingFunctions;

namespace RpgInterpreterTests.ParserTests.Utils;

internal static class TokenEnumerableExtensions
{
    public static SourceState ToSourceState(this IEnumerable<Token> tokens) =>
        new(ImmutableQueue.CreateRange(
            tokens.Select(t => new PositionedToken(t, new Position(0, 0), new Position(0, 0)))));
}