using RpgInterpreter.Lexer.Sources;

namespace RpgInterpreter.Lexer.Tokens;

public record PositionedToken(Token Value, Position Start, Position End) : Token;