using RpgInterpreter;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreterTests.Utils;

internal class AstFactory
{
    public static Position Start { get; set; } = new(0, 0);
    public static Position End { get; set; } = new(0, 0);

    public static Root Root(NodeList<Statement> statements) => new(statements, Start, End);

    public static FunctionDeclaration FunctionDeclaration(string name, FunctionParameterList parameterList,
        string returnType,
        Block body) => new(name, parameterList, returnType, body, Start, End);

    public static FunctionParameterList FunctionParameterList(NodeList<FunctionParameterDeclaration> parameters) =>
        new(parameters, Start, End);

    public static FunctionInvocation FunctionInvocation(string name, IEnumerable<Expression> parameters) =>
        new(name, parameters, Start, End);

    public static FunctionParameterDeclaration FunctionParameter(string name, string type) =>
        new(name, type, Start, End);

    public static Block Block(NodeList<IBlockInner> inner, Expression last) => new(inner, last, Start, End);
    public static Assignment Assignment(IAssignable left, Expression right) => new(left, right, Start, End);
    public static VariableExp Variable(string name) => new(name, Start, End);
    public static UnaryMinus UnaryMinus(Expression val) => new(val, Start, End);
    public static DiceExpression Dice(int count, int max) => new(count, max, Start, End);
    public static AdditionExp AdditionExp(Expression left, Expression right) => new(left, right, Start, End);
    public static SubtractionExp SubtractionExp(Expression left, Expression right) => new(left, right, Start, End);

    public static MultiplicationExp MultiplicationExp(Expression left, Expression right) =>
        new(left, right, Start, End);

    public static DivisionExp DivisionExp(Expression left, Expression right) => new(left, right, Start, End);
    public static FieldReference FieldReference(NameReference obj, string field) => new(obj, field, Start, End);
    public static Natural Natural(int value) => new(value, Start, End);

    public static StringExpression StringExpression(string value) => new(value, Start, End);

    public static IfExpression If(Expression condition, Expression onTrue, Expression onFalse) =>
        new(condition, onTrue, onFalse, Start, End);

    public static GreaterThanExp GreaterThanExp(Expression left, Expression right) => new(left, right, Start, End);
    public static Unit Unit() => new(Start, End);
}