using NUnit.Framework;
using Optional;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreterTests.LexerTests;

internal class RpgLexerTests
{
    private static ListTestData[] _exactTokens =
    {
        new("", Enumerable.Empty<Token>()),
        new(
            "if ((42 * 3d3) == 12) then \"else\" else { fun([1d4, 1d4, 1d2], [], [[], [1], [1, 2]]) + -42 - --7 * \"abc\" / 1d4 }",
            new Token[]
            {
                new If(), new OpenParen(), new OpenParen(), new NaturalLiteral(42), new Multiplication(),
                new DiceLiteral(3, 3), new CloseParen(),
                new Equality(), new NaturalLiteral(12), new CloseParen(), new Then(), new StringLiteral("else"),
                new Else(), new OpenBrace(),
                new LowercaseIdentifier("fun"), new OpenParen(), new OpenBracket(), new DiceLiteral(1, 4),
                new Comma(), new DiceLiteral(1, 4),
                new Comma(), new DiceLiteral(1, 2), new CloseBracket(), new Comma(), new OpenBracket(),
                new CloseBracket(), new Comma(),
                new OpenBracket(), new OpenBracket(), new CloseBracket(), new Comma(), new OpenBracket(),
                new NaturalLiteral(1),
                new CloseBracket(), new Comma(), new OpenBracket(), new NaturalLiteral(1), new Comma(),
                new NaturalLiteral(2), new CloseBracket(),
                new CloseBracket(), new CloseParen(), new Addition(), new Minus(), new NaturalLiteral(42),
                new Minus(), new Minus(), new Minus(),
                new NaturalLiteral(7), new Multiplication(), new StringLiteral("abc"), new Division(),
                new DiceLiteral(1, 4), new CloseBrace()
            }),
        new(
            "if (100 > 9999())\nthen 2/0\nelse true",
            new Token[]
            {
                new If(), new OpenParen(), new NaturalLiteral(100), new Greater(), new NaturalLiteral(9999),
                new OpenParen(), new CloseParen(),
                new CloseParen(), new Then(), new NaturalLiteral(2), new Division(), new NaturalLiteral(0),
                new Else(), new BooleanLiteral(true)
            }
        )
    };

    private readonly RpgLexer _lexer = new();

    [TestCaseSource(nameof(_exactTokens))]
    public void ExactTest(ListTestData data)
    {
        var result = _lexer.Tokenize(data.Source);

        Assert.That(result.Where(x => x is not Whitespace).ToArray(), Is.EqualTo(data.Output));
    }

    [TestCase("04d2 + 2")]
    [TestCase("4d02 + 1")]
    [TestCase("4d2 + 01")]
    [TestCase("4d2 + 01")]
    [TestCase("4d2 + 01")]
    [TestCase("If {\r\n  If: \"If\",\r\n  If: if if() then then() else else(),\r\n  If: [if, then, else]\r\n}")]
    public void ValidPrograms(string program)
    {
        var source = new StringSource(program);
        var result = _lexer.Tokenize(source).ToArray();

        Assert.That(result, Is.Not.Empty);
        Assert.That(source.Peek(), Is.EqualTo(Option.None<char>()),
            () => "There should be no characters left in the source.");
    }

    [TestCase("if \"\\n\"\r\n  then \"abc\"\r\n  else \"xyz     \r\n\r\n\r\n\r\nabc\"")]
    [TestCase("4d\"abc\"")]
    [TestCase(
        "if (1000000000000000000000000000000000000000000000000000 > 999999999999999999999999999999999999999999999d999999999999999999999999999999999999999())\r\nthen 2/0\r\nelse true")]
    [TestCase(
        "if ((42 * 3d3) == 12) then \"5ԛ1ҩ㌠$#Ǭ\" else { fun([1d4, d4, d2], [], [[], [1], [1, 2]]) + -42 - --7 * \"abc\" / 1d4 }\r\n")]
    public void InvalidPrograms(string program)
    {
        var source = new StringSource(program);

        Assert.That(() => _lexer.Tokenize(source).ToArray(), Throws.InstanceOf<LexingException>());
    }
}