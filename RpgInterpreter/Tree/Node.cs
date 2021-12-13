using RpgInterpreter.Parser;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Tree;

public record Node(IReadOnlyList<Node> Children, Symbol Value, Token? token);

public class ParseTree
{
    private readonly Stack<List<Node>> _forest = new();

    public Node? Root { get; private set; }

    public void BeginSubtree()
    {
        _forest.Push(new List<Node>());
    }

    public void AddLeaf(Symbol symbol, Token token)
    {
        var leaf = new Node(new List<Node>(), symbol, token);
        _forest.Peek().Add(leaf);
    }

    public void EndSubtree(Symbol symbol)
    {
        var nodes = _forest.Pop();
        var node = new Node(nodes, symbol, null);

        if (_forest.Any())
        {
            _forest.Peek().Add(node);
        }
        else
        {
            Root = node;
        }
    }
}