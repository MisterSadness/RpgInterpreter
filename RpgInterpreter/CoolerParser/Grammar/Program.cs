using RpgInterpreter.Tokens;

namespace RpgInterpreter.CoolerParser.Grammar;

public record Root(IEnumerable<Statement> Statements);

public abstract record Statement;

public abstract record BlockInnerStatement : Statement;

public abstract record Expression;

public record FieldDeclaration(UppercaseIdentifier Name, Expression Value);

// Statements
public record ObjectDeclaration(UppercaseIdentifier Name, UppercaseIdentifier Base, TraitList Traits,
        IEnumerable<FieldDeclaration> Fields)
    : Statement;

public record Assignment(LowercaseIdentifier Target, Expression Value) : BlockInnerStatement;

public record FunctionInvocationStatement(FunctionInvocation FunctionInvocation) : BlockInnerStatement;

public record TraitDeclaration(UppercaseIdentifier Name, UppercaseIdentifier Base) : Statement;

public record FunctionDeclaration(LowercaseIdentifier Name, FunctionParameterList ParameterList,
    UppercaseIdentifier ReturnType, Block Body) : Statement;

public record FunctionParameterList(IEnumerable<FunctionParameter> Parameters);

public record FunctionParameter(LowercaseIdentifier Name, UppercaseIdentifier Type);

// Expressions
public record List(IEnumerable<Expression> Elements) : Expression;

public record Unit : Expression;

public record Natural(uint Value) : Expression;

public record Dice(uint Count, int Min, int Max) : Expression;

public record String(string Value) : Expression;

public record Boolean(bool Value) : Expression;

public record UnaryMinus(Expression Value) : Expression;

public record If(Expression Condition, Expression TrueValue, Expression FalseValue) : Expression;

// If Block is only statements then Last is set to the Unit expression.
public record Block(IEnumerable<BlockInnerStatement> Inner, Expression Last) : Expression;

public record ObjectCreation(UppercaseIdentifier Type, TraitList Traits) : Expression;

public record TraitList(IEnumerable<Trait> Traits, IEnumerable<FieldDeclaration> Fields) : Expression;

public record Variable(LowercaseIdentifier Name) : Expression;

public record FieldReference(LowercaseIdentifier ObjectName, UppercaseIdentifier FieldName) : Expression;

public record FunctionInvocation(LowercaseIdentifier FunctionName, IEnumerable<Expression> Arguments) : Expression;

public record DiceRoll(Expression Dice) : Expression;

public abstract record BinaryOperation(Expression Left, Expression Right) : Expression;

public record Addition(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record Subtraction(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record Multiplication(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record Division(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record BooleanAnd(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record BooleanOr(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record Concatenation(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record Equal(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record NotEqual(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record LessThan(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record GreaterThan(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record LessOrEqualThan(Expression Left, Expression Right) : BinaryOperation(Left, Right);

public record GreaterOrEqualThan(Expression Left, Expression Right) : BinaryOperation(Left, Right);


