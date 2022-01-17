using RpgInterpreter.Lexer.Sources;

namespace RpgInterpreter.CoolerParser.Grammar;

public record Node(Position Start, Position End) : IPositioned;

public record Root(NodeList<Statement> Statements, Position Start, Position End) : Node(Start, End);

public record FieldDeclaration(string Name, Expression Value, Position Start, Position End) : Node(Start, End);

public record FieldList(NodeList<FieldDeclaration> Fields, Position Start, Position End) : Node(Start, End);

public record FunctionParameterList(NodeList<FunctionParameter> Parameters, Position Start, Position End) : Node(Start,
    End);

public record FunctionParameter(string Name, string Type, Position Start, Position End) : Node(Start, End);
