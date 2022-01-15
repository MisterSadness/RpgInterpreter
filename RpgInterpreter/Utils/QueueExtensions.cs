using System.Collections.Immutable;
using RpgInterpreter.CoolerParser;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Utils;

public static class QueueExtensions
{
    public static SourceState ToState(this IImmutableQueue<Token> queue) => new(queue);

    public static T? PeekOrDefault<T>(this IImmutableQueue<T> queue) => queue.IsEmpty ? default : queue.Peek();
}