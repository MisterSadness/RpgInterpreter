using NUnit.Framework;
using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Tokens;

namespace RpgInterpreterTests.LexerTests
{
    internal class StringLexerTests
    {
        private static SingleTestData[] _strings =
        {
            new("\"\"", new StringLiteral("")),
            new("\"\\t\\n\"", new StringLiteral("\t\n")),
            new("\"Aaaaaaa\"", new StringLiteral("Aaaaaaa")),
            new("\"Jan_Kowalski_99\"", new StringLiteral("Jan_Kowalski_99")),
            new("\"new\\nline\"", new StringLiteral("new\nline")),
            new("\"backslash\\\\\"", new StringLiteral("backslash\\")),
            new("\"quote\\\"quote\"", new StringLiteral("quote\"quote"))
        };

        private readonly StringLexer _lexer = new();

        [TestCaseSource(nameof(_strings))]
        public void StringTest(SingleTestData data)
        {
            var result = _lexer.Match(data.Source);

            Assert.That(result, Is.EqualTo(data.Output));
        }
    }
}