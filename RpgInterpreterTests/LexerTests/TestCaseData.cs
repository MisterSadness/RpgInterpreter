using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreterTests.LexerTests;

public record SingleTestData(string Input, Token Output)
{
    public StringSource Source => new(Input);
}

public record ListTestData(string Input, IEnumerable<Token> Output)
{
    public StringSource Source => new(Input);
}