using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Tokens;

namespace RpgInterpreterTests.LexerTests
{
    class SeparatorLexerTests
    {
        private readonly SeparatorLexer _innerLexer = new();
        private readonly Lexer _lexer = new(new InnerLexer[] { new SeparatorLexer() });

        [TestCaseSource(nameof(_singleSeparator))]
        public void SingleSeparatorTest(SingleTestData data)
        {
            var result = _innerLexer.Match(data.Source);

            Assert.That(result, Is.EqualTo(data.Output));
        }

        private static SingleTestData[] _singleSeparator = {
            new ("(", new OpenParen()),
            new (")", new CloseParen()),
            new ("[", new OpenBracket()),
            new ("]", new CloseBracket()),
            new ("{", new OpenBrace()),
            new ("}", new CloseBrace()),
            new (":", new Colon()),
            new (",", new Comma()),
        };

        [TestCaseSource(nameof(_separatorList))]
        public void RandomSeparatorListTest(ListTestData data)
        {
            var result = _lexer.Tokenize(data.Source);

            Assert.That(result, Is.EqualTo(data.Output));
        }

        private static ListTestData[] _separatorList = {
            new ("()(((", new Separator[]
            {
                new OpenParen(), new CloseParen(), new OpenParen(), new OpenParen(), new OpenParen()
            }),
            new (")[}{", new Separator[]
            {
                new CloseParen(), new OpenBracket(), new CloseBrace(), new OpenBrace()
            }),
            new ("][::][", new Separator[]
            {
                new CloseBracket(), new OpenBracket(), new Colon(), new Colon(), new CloseBracket(), new OpenBracket()
            }),
        };
    }
}
