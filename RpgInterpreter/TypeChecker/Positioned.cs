using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.TypeChecker;

public record Positioned(Position Start, Position End) : IPositioned;