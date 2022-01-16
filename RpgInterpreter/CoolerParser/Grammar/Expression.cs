namespace RpgInterpreter.CoolerParser.Grammar;

public interface IValue { }

public abstract record Expression : IBlockInner;

public record List(NodeList<Expression> Elements) : Expression;

public record Unit : Expression;

public record Natural(int Value) : Expression;

public record Dice(int Count, int Max) : Expression;

public record StringExpression(string Value) : Expression;

public record BooleanExpression(bool Value) : Expression;

public record UnaryMinus(Expression Value) : Expression;

public record If(Expression Condition, Expression TrueValue, Expression FalseValue) : Expression;

// If Block is only statements then Last is set to the Unit expression.
public record Block(NodeList<IBlockInner> Inner, Expression Last) : Expression;

public record ObjectCreation(string Type, TraitList Traits) : Expression;

public record TraitList(NodeList<string> Traits) : Expression;

public abstract record Reference : Expression;

public interface IAssignable { }

public abstract record NameReference : Reference, IAssignable;

public record Variable(string Name) : NameReference;

public record Base : NameReference;

public record This : NameReference;

public record FieldReference(NameReference ObjectName, string FieldName) : Reference, IAssignable;

public record FunctionInvocation(string FunctionName, IEnumerable<Expression> Arguments) : Reference;

public record DiceRoll(Expression Dice) : Expression;

public enum Precedence
{
    None = 0,
    Boolean = 1,
    Comparison = 2,
    Addition = 3,
    Concatenation = 4,
    Multiplication = 5,
    UnaryMinus = 6
}

public abstract record BinaryOperation(Expression Left, Expression Right, Precedence Precedence) : Expression;

public record AdditionExp(Expression Left, Expression Right) : BinaryOperation(Left, Right, Precedence.Addition);

public record SubtractionExp(Expression Left, Expression Right) : BinaryOperation(Left, Right, Precedence.Addition);

public record MultiplicationExp(Expression Left, Expression Right) : BinaryOperation(Left, Right,
    Precedence.Multiplication);

public record DivisionExp(Expression Left, Expression Right) : BinaryOperation(Left, Right, Precedence.Multiplication);

public record BooleanAndExp(Expression Left, Expression Right) : BinaryOperation(Left, Right, Precedence.Boolean);

public record BooleanOrExp(Expression Left, Expression Right) : BinaryOperation(Left, Right, Precedence.Boolean);

public record ConcatenationExp(Expression Left, Expression Right) : BinaryOperation(Left, Right,
    Precedence.Concatenation);

public record EqualExp(Expression Left, Expression Right) : BinaryOperation(Left, Right, Precedence.Comparison);

public record NotEqualExp(Expression Left, Expression Right) : BinaryOperation(Left, Right, Precedence.Comparison);

public record LessThanExp(Expression Left, Expression Right) : BinaryOperation(Left, Right, Precedence.Comparison);

public record GreaterThanExp(Expression Left, Expression Right) : BinaryOperation(Left, Right, Precedence.Comparison);

public record LessOrEqualThanExp(Expression Left, Expression Right) : BinaryOperation(Left, Right,
    Precedence.Comparison);

public record GreaterOrEqualThanExp(Expression Left, Expression Right) : BinaryOperation(Left, Right,
    Precedence.Comparison);
