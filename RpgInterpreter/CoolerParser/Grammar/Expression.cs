using RpgInterpreter.Tokens;

namespace RpgInterpreter.CoolerParser.Grammar;

public abstract record Expression : IBlockInner;

public record List(IEnumerable<Expression> Elements) : Expression;

public record Unit : Expression;

public record Natural(NaturalLiteral Value) : Expression;

public record Dice(DiceLiteral Value) : Expression;

public record StringExpression(StringLiteral Value) : Expression;

public record BooleanExpression(BooleanLiteral Value) : Expression;

public record UnaryMinus(Expression Value) : Expression;

public record If(Expression Condition, Expression TrueValue, Expression FalseValue) : Expression;

// If Block is only statements then Last is set to the Unit expression.
public record Block(IEnumerable<IBlockInner> Inner, Expression Last) : Expression
{
    public override int GetHashCode()
    {
        var hash = new HashCode();
        
        foreach (var inner in Inner)
        {
            hash.Add(inner);
        }

        hash.Add(Last);

        return hash.ToHashCode();
    }

    public virtual bool Equals(Block other) => base.Equals(other) && Inner.SequenceEqual(other.Inner) && Last.Equals(other.Last);
}

public record ObjectCreation(UppercaseIdentifier Type, TraitList Traits) : Expression;

public record TraitList(IEnumerable<UppercaseIdentifier> Traits) : Expression;

public abstract record Reference : Expression;

public abstract record NameReference : Reference;

public record Variable(LowercaseIdentifier Name) : NameReference;

public record Base : NameReference;

public record This : NameReference;

public record FieldReference(NameReference ObjectName, UppercaseIdentifier FieldName) : Reference;

public record FunctionInvocation(LowercaseIdentifier FunctionName, IEnumerable<Expression> Arguments) : Reference;

public record DiceRoll(Expression Dice) : Expression;

public record Parentheses(Expression Inner) : Expression;

public abstract record BinaryOperation(Expression Left, Expression Right) : Expression;

public record AdditionExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record SubtractionExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record MultiplicationExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record DivisionExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record BooleanAndExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record BooleanOrExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record ConcatenationExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record EqualExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record NotEqualExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record LessThanExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record GreaterThanExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record LessOrEqualThanExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record GreaterOrEqualThanExp(Expression Left, Expression Right) : BinaryOperation(Left, Right);
