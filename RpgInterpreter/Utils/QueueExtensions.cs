using System.Collections.Immutable;
using RpgInterpreter.CoolerParser.ParsingFunctions;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.Utils;

public static class QueueExtensions
{
    public static SourceState ToState(this IImmutableQueue<PositionedToken> queue) => new(queue);

    public static T? PeekOrDefault<T>(this IImmutableQueue<T> queue) => queue.IsEmpty ? default : queue.Peek();
}