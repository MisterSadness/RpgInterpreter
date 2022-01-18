namespace RpgInterpreter.Parser.Grammar;

public abstract record Statement(Position Start, Position End) : Node(Start, End);

public interface IBlockInner { }

public record ObjectDeclaration(string Name, string? Base, TraitList? Traits,
        FieldList Fields, Position Start, Position End)
    : Statement(Start, End);

public record TraitDeclaration
    (string Name, string? Base, FieldList Fields, Position Start, Position End) : Statement(Start, End);

public record Assignment(IAssignable Target, Expression Value, Position Start, Position End) : Statement(Start, End),
    IBlockInner;

public record FunctionInvocationStatement
    (FunctionInvocation FunctionInvocation, Position Start, Position End) : Statement(Start, End);

public record FunctionDeclaration(string Name, FunctionParameterList ParameterList,
    string ReturnType, Block Body, Position Start, Position End) : Statement(Start, End);