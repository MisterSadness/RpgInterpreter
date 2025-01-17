﻿using NUnit.Framework;
using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreterTests.LexerTests;

internal class LowercaseWordLexerTests
{
    private readonly LowercaseWordLexer _lexer = new();

    [TestCaseSource(nameof(_lowercaseWords))]
    public void LowercaseWordTest(SingleTestData data)
    {
        var result = _lexer.Match(data.Source);

        Assert.That(result, Is.EqualTo(data.Output));
    }

    private static SingleTestData[] _lowercaseWords =
    {
        new("aA321A1", new LowercaseIdentifier("aA321A1")),
        new("witch", new LowercaseIdentifier("witch")),
        new("aaaa_aaAADA1321", new LowercaseIdentifier("aaaa_aaAADA1321")),
        new("if", new If()),
        new("then", new Then()),
        new("else", new Else()),
        new("trait", new Trait()),
        new("for", new For()),
        new("with", new With()),
        new("new", new New()),
        new("extends", new Extends()),
        new("and", new And()),
        new("base", new Base()),
        new("this", new This()),
        new("set", new Set()),
        new("fun", new Fun())
    };
}