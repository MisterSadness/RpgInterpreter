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
    class StringLexerTests
    {
        private readonly StringLexer _lexer = new();

        [TestCaseSource(nameof(_strings))]
        public void StringTest(SingleTestData data)
        {
            var result = _lexer.Match(data.Source);

            Assert.That(result, Is.EqualTo(data.Output));
        }

        private static SingleTestData[] _strings = {
            new ("\"\"", new StringLiteral("")),
            new ("\"Aaaaaaa\"", new StringLiteral("Aaaaaaa")),
            new ("\"Jan_Kowalski_99\"", new StringLiteral("Jan_Kowalski_99")),
            new ("\"new\\nline\"", new StringLiteral("new\nline")),
            new ("\"backslash\\\\\"", new StringLiteral("backslash\\")),
            new ("\"quote\\\"quote\"", new StringLiteral("quote\"quote")),
        };
    }
}
