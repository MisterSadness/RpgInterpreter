using System.Collections.Immutable;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.TypeChecker;

public record Type
{
    public virtual bool IsAssignableTo(Type right)
    {
        return (this, right) switch
        {
            (ObjectType o1, ObjectType o2) => o1.IsAssignableTo(o2),
            _ => this == right
        };
    }
}

public record UnitType : Type
{
    public override string ToString() => "Unit";
}

public record StringType : Type
{
    public override string ToString() => "String";
}

public record BooleanType : Type
{
    public override string ToString() => "Bool";
}

public record IntType : Type
{
    public override string ToString() => "Int";
}

public record DiceType : Type
{
    public override string ToString() => "Dice";
}

public abstract record ListType : Type { }

public record EmptyListType(IPositioned Positioned) : ListType
{
    public override string ToString() => "List[?unknown?]";

    public override bool IsAssignableTo(Type right)
    {
        return right switch
        {
            EmptyListType => true,
            _ => false
        };
    }
}

public record TypedListType(Type ElementType) : ListType
{
    public override string ToString() => $"List[{ElementType}]";

    public override bool IsAssignableTo(Type right)
    {
        return right switch
        {
            TypedListType t when ElementType.IsAssignableTo(t.ElementType) => true,
            EmptyListType => true,
            _ => false
        };
    }
}

public record FunctionType(IImmutableList<FunctionParameter> ParameterTypes, Type ReturnType, string Name) : Type
{
    public override string ToString() => $"({string.Join(", ", ParameterTypes)}) => {ReturnType}";
}

public record ObjectType(IImmutableDictionary<string, Type> FieldTypes, string TypeName,
    IImmutableSet<ObjectType> BaseTypes) : Type
{
    public override string ToString() => TypeName;

    public override bool IsAssignableTo(Type right)
    {
        return right switch
        {
            ObjectType o when o.TypeName == TypeName => true,
            ObjectType o => BaseTypes.Any(t => t.TypeName == o.TypeName),
            _ => false
        };
    }
}