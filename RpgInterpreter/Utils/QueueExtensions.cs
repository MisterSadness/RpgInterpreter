using System.Collections.Immutable;

namespace RpgInterpreter.Utils;

public static class QueueExtensions
{
    public static T? PeekOrDefault<T>(this IImmutableQueue<T> queue) => queue.IsEmpty ? default : queue.Peek();
}