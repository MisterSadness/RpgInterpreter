﻿using NUnit.Framework;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreterTests.Utils;
using Assignment = RpgInterpreter.Lexer.Tokens.Assignment;

namespace RpgInterpreterTests.ParserTests;

public class BlockTests
{
    [Test]
    public void ParseBlock_CanContainAssignment()
    {
        var tokensInBlock = new Token[]
        {
            new OpenBrace(), new Set(), new LowercaseIdentifier("x"), new Assignment(), new NaturalLiteral(42),
            new Semicolon(), new CloseBrace(), new LexingFinished()
        };
        var source = tokensInBlock.ToSourceState();
        var expectedTree =
            AstFactory.Block(NodeList.From(new IBlockInner[]
            {
                AstFactory.Assignment(AstFactory.Variable("x"), AstFactory.Natural(42))
            }), AstFactory.Unit());

        var parsed = source.ParseBlock();

        Assert.That(parsed.Source.PeekOrDefault(), Is.TypeOf<LexingFinished>());
        Assert.AreEqual(expectedTree, parsed.Result);
    }
}