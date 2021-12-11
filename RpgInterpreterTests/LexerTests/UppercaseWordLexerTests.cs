using NUnit.Framework;
using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Tokens;

namespace RpgInterpreterTests.LexerTests;

internal class UppercaseWordLexerTests
{
    private static SingleTestData[] _uppercaseWords =
    {
        new("Aaaaaaa", new UppercaseIdentifier("Aaaaaaa")),
        new("Jan_Kowalski_99", new UppercaseIdentifier("Jan_Kowalski_99")),
        new("A11A1Aaa1_", new UppercaseIdentifier("A11A1Aaa1_"))
    };

    private readonly UppercaseWordLexer _lexer = new();

    [TestCaseSource(nameof(_uppercaseWords))]
    public void UppercaseWordTest(SingleTestData data)
    {
        var result = _lexer.Match(data.Source);

        Assert.That(result, Is.EqualTo(data.Output));
    }
}