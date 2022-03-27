using System.Collections;
using System.Text;

namespace RpgInterpreter.Parser.Grammar;

public static class NodeList
{
    public static NodeList<T> From<T>(IEnumerable<T> nodes) where T : notnull => new(nodes);
}

public sealed class NodeList<T> : IReadOnlyCollection<T>, IEquatable<NodeList<T>> where T : notnull
{
    private readonly IReadOnlyCollection<T> _nodes;

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is NodeList<T> other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var element in _nodes)
        {
            hash.Add(element);
        }

        return hash.ToHashCode();
    }

    public NodeList(IEnumerable<T> nodes) => _nodes = nodes.ToList();

    public IEnumerator<T> GetEnumerator() => _nodes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_nodes).GetEnumerator();

    public int Count => _nodes.Count;

    public bool Equals(NodeList<T>? other) => other switch
    {
        null => false,
        { } => ReferenceEquals(this, other) || ReferenceEquals(_nodes, other._nodes) ||
               _nodes.SequenceEqual(other._nodes)
    };

    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var node in _nodes)
        {
            sb.Append(node);
        }

        return sb.ToString();
    }
}