namespace RpgInterpreter.Parser.Grammar;

public interface IWithScope { }

public record Node(Position Start, Position End) : IPositioned;

public record Root(NodeList<Statement> Statements, Position Start, Position End) : Node(Start, End), IWithScope;

public record FieldDeclaration(string Name, Expression Value, Position Start, Position End) : Node(Start, End);

public record FieldList(NodeList<FieldDeclaration> Fields, Position Start, Position End) : Node(Start, End);

public record FunctionParameterList(NodeList<FunctionParameterDeclaration> Parameters, Position Start, Position End) :
    Node(Start, End);

public record FunctionParameterDeclaration(string Name, string Type, Position Start, Position End) : Node(Start, End);
