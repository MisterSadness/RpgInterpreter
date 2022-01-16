namespace RpgInterpreter.CoolerParser.Grammar;

public abstract record Statement;

public interface IBlockInner { }

public record ObjectDeclaration(string Name, string? Base, TraitList? Traits,
        FieldList Fields)
    : Statement;

public record TraitDeclaration(string Name, string? Base, FieldList Fields) : Statement;

public record Assignment(IAssignable Target, Expression Value) : Statement, IBlockInner;

public record FunctionInvocationStatement(FunctionInvocation FunctionInvocation) : Statement;

public record FunctionDeclaration(string Name, FunctionParameterList ParameterList,
    string ReturnType, Block Body) : Statement;