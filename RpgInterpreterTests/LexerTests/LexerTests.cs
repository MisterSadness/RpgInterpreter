using System.Linq;
using NUnit.Framework;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;

namespace RpgInterpreterTests.LexerTests;

internal class LexerTests
{
    [Test]
    public void ThrowsExceptionOnInvalidInput()
    {
        var lexer = new Lexer(Enumerable.Empty<InnerLexer>());
        var source = new StringSource("aaaa");

        Assert.That(() => lexer.Tokenize(source).ToArray(), Throws.TypeOf<UnexpectedInputException>());
    }
}