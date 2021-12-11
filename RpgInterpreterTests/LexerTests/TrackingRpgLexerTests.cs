using NUnit.Framework;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;

namespace RpgInterpreterTests.LexerTests;

internal class TrackingRpgLexerTests
{
    private readonly TrackingRpgLexer _lexer = new();

    [TestCase("$", 0, 0)]
    [TestCase("1d4 + 3d\"aaaa", 0, 8)]
    [TestCase("1d4\n3d\"aaaa\n2d6", 1, 2)]
    [TestCase("\"5ԛ1㌠\"", 0, 5)]
    public void ReportsPositionCorrectly(string program, int line, int column)
    {
        try
        {
            var source = new StringSource(program);

            var _ = _lexer.Tokenize(source).ToArray();
        }
        catch (PositionedLexingException exception)
        {
            Assert.That(exception.Position, Is.EqualTo(new Position(line, column)));
            return;
        }

        Assert.Fail("No exception occurred.");
    }
}