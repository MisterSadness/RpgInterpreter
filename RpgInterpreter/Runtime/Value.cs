using RpgInterpreter.TypeChecker;
using Type = RpgInterpreter.TypeChecker.Type;

namespace RpgInterpreter.Runtime;

public abstract record Value : ITyped
{
    public abstract string PrintableString { get; }
    public abstract Type Type { get; }
}

public interface ITyped
{
    Type Type { get; }
}

public record Integer(int Value) : Value
{
    public override string PrintableString => Value.ToString();
    public override Type Type => new IntType();
}

public abstract record Dice : Value
{
    protected static readonly Random Random = new();
    public override Type Type => new DiceType();
    public abstract int Roll();
}

public record SimpleDice(int Count, int Max) : Dice
{
    public override string PrintableString => $"{Count}d{Max}";
    public override int Roll() => Enumerable.Range(1, Count).Sum(_ => Random.Next(1, Max + 1));
}

public record CompoundDice(Func<int> RollFunc) : Dice
{
    public override string PrintableString => "Dice";
    public override int Roll() => RollFunc();
}

public record Unit : Value
{
    public override string PrintableString => "()";
    public override Type Type => new UnitType();
}

public record String(string Value) : Value
{
    public override string PrintableString => $"{Value.ReplaceLineEndings()}";
    public override Type Type => new StringType();
}

public record Boolean(bool Value) : Value
{
    public override string PrintableString => Value.ToString().ToLower();
    public override Type Type => new BooleanType();
}