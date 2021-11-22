using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreterTests.LexerTests
{
    class WhitespaceLexerTests
    {
        private readonly WhitespaceLexer _lexer = new();

        [TestCaseSource(nameof(_whitespace))]
        public void WhitespaceTest(SingleTestData data)
        {
            var result = _lexer.Match(data.Source);

            Assert.That(result, Is.EqualTo(data.Output));
        }

        private static SingleTestData[] _whitespace = {
            new ("  ", new Whitespace()),
            new (" \t  \n\n\t  ", new Whitespace()),
            new ("\t", new Whitespace()),
            new ("\n", new Whitespace()),
            new ("    \t\n", new Whitespace()),
        };

        [Test]
        public void DoesNotAcceptEmptyString()
        {
            var source = new StringSource("");

            Assert.Throws<UnexpectedInputException>(() => _lexer.Match(source));
        }
    }
}
