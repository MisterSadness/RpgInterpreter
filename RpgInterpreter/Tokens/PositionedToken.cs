using RpgInterpreter.Lexer.Sources;

namespace RpgInterpreter.Tokens;

public record PositionedToken(Token Value, Position Start, Position End) : Token;