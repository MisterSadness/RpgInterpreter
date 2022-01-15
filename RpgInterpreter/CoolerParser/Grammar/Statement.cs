using RpgInterpreter.Tokens;

namespace RpgInterpreter.CoolerParser.Grammar;

public abstract record Statement;

public interface IBlockInner { }

public record ObjectDeclaration(UppercaseIdentifier Name, UppercaseIdentifier? Base, TraitList? Traits,
        FieldList Fields)
    : Statement;

public record TraitDeclaration(UppercaseIdentifier Name, UppercaseIdentifier? Base, FieldList Fields) : Statement;

public record Assignment(LowercaseIdentifier Target, Expression Value) : Statement, IBlockInner;

public record FunctionInvocationStatement(FunctionInvocation FunctionInvocation) : Statement;

public record FunctionDeclaration(LowercaseIdentifier Name, FunctionParameterList ParameterList,
    UppercaseIdentifier ReturnType, Block Body) : Statement;