using System.Collections.Immutable;
using System.Text;
using RpgInterpreter.TypeChecker;
using Type = RpgInterpreter.TypeChecker.Type;

namespace RpgInterpreter.Runtime;

public record Object(IImmutableDictionary<string, Value> Fields, ObjectType ObjectType) : Value
{
    public override string PrintableString => ConstructPrintableString();

    private string ConstructPrintableString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{ObjectType} {{");
        foreach (var (name, value) in Fields.OrderBy(kvp => kvp.Key))
        {
            sb.AppendLine($"  {name}: {value.PrintableString}");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    public override Type Type => ObjectType;
}