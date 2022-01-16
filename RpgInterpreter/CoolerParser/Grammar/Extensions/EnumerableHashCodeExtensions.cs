namespace RpgInterpreter.CoolerParser.Grammar.Extensions;

internal static class EnumerableHashCode
{
    public static int GetSequenceHashCode<T>(this IEnumerable<T> source)
    {
        var hash = new HashCode();

        foreach (var element in source)
        {
            hash.Add(element);
        }

        return hash.ToHashCode();
    }
}