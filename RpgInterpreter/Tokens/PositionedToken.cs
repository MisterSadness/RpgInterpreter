using RpgInterpreter.Lexer.Sources;

namespace RpgInterpreter.Tokens;

public record PositionedToken(Token Token, Position Position) : Token;