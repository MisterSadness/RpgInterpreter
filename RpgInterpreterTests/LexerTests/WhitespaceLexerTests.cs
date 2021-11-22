using NUnit.Framework;
using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreterTests.LexerTests
{
    internal class WhitespaceLexerTests
    {
        private static SingleTestData[] _whitespace =
        {
            new("  ", new Whitespace()),
            new(" \t  \n\n\t  ", new Whitespace()),
            new("\t", new Whitespace()),
            new("\n", new Whitespace()),
            new("    \t\n", new Whitespace())
        };

        private readonly WhitespaceLexer _lexer = new();

        [TestCaseSource(nameof(_whitespace))]
        public void WhitespaceTest(SingleTestData data)
        {
            var result = _lexer.Match(data.Source);

            Assert.That(result, Is.EqualTo(data.Output));
        }

        [Test]
        public void DoesNotAcceptEmptyString()
        {
            var source = new StringSource("");

            Assert.Throws<UnexpectedInputException>(() => _lexer.Match(source));
        }
    }
}