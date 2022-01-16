namespace RpgInterpreter.CoolerParser.Grammar;

public record Root(NodeList<Statement> Statements);

public record FieldDeclaration(string Name, Expression Value);

public record FieldList(NodeList<FieldDeclaration> Fields);

public record FunctionParameterList(NodeList<FunctionParameter> Parameters);

public record FunctionParameter(string Name, string Type);
