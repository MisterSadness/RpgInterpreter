using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreterTests.LexerTests
{
    class UppercaseWordLexerTests
    {
        private readonly UppercaseWordLexer _lexer = new();

        [TestCaseSource(nameof(_uppercaseWords))]
        public void UppercaseWordTest(SingleTestData data)
        {
            var result = _lexer.Match(data.Source);

            Assert.That(result, Is.EqualTo(data.Output));
        }

        private static SingleTestData[] _uppercaseWords = {
            new ("Aaaaaaa", new UppercaseIdentifier("Aaaaaaa")),
            new ("Jan_Kowalski_99", new UppercaseIdentifier("Jan_Kowalski_99")),
            new ("A11A1Aaa1_", new UppercaseIdentifier("A11A1Aaa1_"))
        };
    }
}
