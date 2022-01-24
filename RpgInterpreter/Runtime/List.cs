using System.Collections.Immutable;
using System.Text;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreter.TypeChecker;
using Type = RpgInterpreter.TypeChecker.Type;

namespace RpgInterpreter.Runtime;

// public abstract record List(Dictionary<string, Value> Fields, ListType ListType) : Object(Fields, ListType)
// {
//     public abstract List ConcatOrThrow(List other);
// }

public record EmptyList(IPositioned Positioned) : Value
{
    public override string PrintableString => "[]";

    public override Type Type => new EmptyListType(Positioned);
}

public record TypedList(IImmutableList<Value> Elements, Type ElementType) : Value
{
    public override Type Type => new TypedListType(ElementType);

    public override string PrintableString => ConstructPrintableString();

    private string ConstructPrintableString()
    {
        var sb = new StringBuilder();
        sb.Append('[');
        sb.AppendJoin(", ", Elements.Select(e => e.PrintableString));
        sb.Append(']');
        return sb.ToString();
    }
}