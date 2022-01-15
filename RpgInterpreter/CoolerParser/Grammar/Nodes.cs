using RpgInterpreter.Tokens;

namespace RpgInterpreter.CoolerParser.Grammar;

public record Root(IEnumerable<Statement> Statements);

public record FieldDeclaration(UppercaseIdentifier Name, Expression Value);

public record FieldList(IEnumerable<FieldDeclaration> Fields);

public record FunctionParameterList(IEnumerable<FunctionParameter> Parameters);

public record FunctionParameter(LowercaseIdentifier Name, UppercaseIdentifier Type);
