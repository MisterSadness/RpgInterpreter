using NUnit.Framework;
using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Tokens;

namespace RpgInterpreterTests.LexerTests;

internal class OperatorLexerTests
{
    private readonly OperatorLexer _lexer = new();

    [TestCaseSource(nameof(_operators))]
    public void OperatorTest(SingleTestData data)
    {
        var result = _lexer.Match(data.Source);

        Assert.That(result, Is.EqualTo(data.Output));
    }

    private static SingleTestData[] _operators =
    {
        new("+", new Addition()),
        new("-", new Minus()),
        new("*", new Multiplication()),
        new("/", new Division()),
        new("=", new Assignment()),
        new("==", new Equality()),
        new("!=", new Inequality()),
        new(">", new Greater()),
        new("<", new Less()),
        new(">=", new GreaterOrEqual()),
        new("<=", new LessOrEqual()),
        new("&&", new BooleanAnd()),
        new("||", new BooleanOr()),
        new("++", new Concatenation()),
        new(".", new Access())
    };
}