using System.Collections.Immutable;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.ParsingFunctions;

namespace RpgInterpreter.Utils;

public static class QueueExtensions
{
    public static SourceState ToState(this IImmutableQueue<PositionedToken> queue) => new(queue);

    public static T? PeekOrDefault<T>(this IImmutableQueue<T> queue) => queue.IsEmpty ? default : queue.Peek();
}