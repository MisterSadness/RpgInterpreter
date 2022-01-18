namespace RpgInterpreter.Parser.Grammar;

public abstract record Expression(Position Start, Position End) : Node(Start, End), IBlockInner;

public record List(NodeList<Expression> Elements, Position Start, Position End) : Expression(Start, End);

public record Unit(Position Start, Position End) : Expression(Start, End);

public record Natural(int Value, Position Start, Position End) : Expression(Start, End);

public record Dice(int Count, int Max, Position Start, Position End) : Expression(Start, End);

public record StringExpression(string Value, Position Start, Position End) : Expression(Start, End);

public record BooleanExpression(bool Value, Position Start, Position End) : Expression(Start, End);

public record UnaryMinus(Expression Value, Position Start, Position End) : Expression(Start, End);

public record If
    (Expression Condition, Expression TrueValue, Expression FalseValue, Position Start, Position End) : Expression(
        Start, End);

// If Block is only statements then Last is set to the Unit expression.
public record Block
    (NodeList<IBlockInner> Inner, Expression Last, Position Start, Position End) : Expression(Start, End);

public record ObjectCreation(string Type, TraitList? Traits, Position Start, Position End) : Expression(Start, End);

public record TraitList(NodeList<string> Traits, Position Start, Position End) : Expression(Start, End);

public abstract record Reference(Position Start, Position End) : Expression(Start, End);

public interface IAssignable { }

public abstract record NameReference(Position Start, Position End) : Reference(Start, End), IAssignable;

public record Variable(string Name, Position Start, Position End) : NameReference(Start, End);

public record Base(Position Start, Position End) : NameReference(Start, End);

public record This(Position Start, Position End) : NameReference(Start, End);

public record FieldReference
    (NameReference ObjectName, string FieldName, Position Start, Position End) : Reference(Start, End), IAssignable;

public record FunctionInvocation
    (string FunctionName, IEnumerable<Expression> Arguments, Position Start, Position End) : Reference(Start, End);

public record DiceRoll(Expression Dice, Position Start, Position End) : Expression(Start, End);

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

public abstract record BinaryOperation(Expression Left, Expression Right, Precedence Precedence, Position Start,
    Position End) : Expression(Start, End);

public record AdditionExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(Left,
    Right, Precedence.Addition, Start, End);

public record SubtractionExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(Left,
    Right, Precedence.Addition, Start, End);

public record MultiplicationExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(Left,
    Right,
    Precedence.Multiplication, Start, End);

public record DivisionExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(Left,
    Right, Precedence.Multiplication, Start, End);

public record BooleanAndExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(Left,
    Right, Precedence.Boolean, Start, End);

public record BooleanOrExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(Left,
    Right, Precedence.Boolean, Start, End);

public record ConcatenationExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(Left,
    Right,
    Precedence.Concatenation, Start, End);

public record EqualExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(Left, Right,
    Precedence.Comparison, Start, End);

public record NotEqualExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(Left,
    Right, Precedence.Comparison, Start, End);

public record LessThanExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(Left,
    Right, Precedence.Comparison, Start, End);

public record GreaterThanExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(Left,
    Right, Precedence.Comparison, Start, End);

public record LessOrEqualThanExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(
    Left, Right,
    Precedence.Comparison, Start, End);

public record GreaterOrEqualThanExp(Expression Left, Expression Right, Position Start, Position End) : BinaryOperation(
    Left, Right,
    Precedence.Comparison, Start, End);
