using NUnit.Framework;
using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser;
using RpgInterpreterTests.CoolerParserTests.Extensions;
using Assignment = RpgInterpreter.Lexer.Tokens.Assignment;

namespace RpgInterpreterTests.CoolerParserTests;

internal class AssignmentTests
{
    public record struct TestData(IEnumerable<Token> Tokens, object ExpectedTree);

    private static IEnumerable<TestData> _assignmentData = new TestData[]
    {
        new(
            new Token[] { new Set(), new LowercaseIdentifier("x"), new Assignment(), new NaturalLiteral(42) },
            new RpgInterpreter.CoolerParser.Grammar.Assignment(new Variable("x"),
                new Natural(42))
        )
    };

    [Test]
    public void WithoutSetKeyword_Fails()
    {
        var tokens = new Token[] { new LowercaseIdentifier("x"), new Assignment(), new NaturalLiteral(42) };
        var source = tokens.ToSourceState();

        Assert.Throws<ParsingException>(() => source.ParseAssignment());
    }

    [TestCaseSource(nameof(_assignmentData))]
    public void ParseAssignment_CreatesParseTree(TestData data)
    {
        var source = data.Tokens.ToSourceState();

        var parsed = source.ParseAssignment();

        Assert.IsEmpty(parsed.Source.Queue);
        Assert.AreEqual(data.ExpectedTree, parsed.Result);
    }

    [TestCaseSource(nameof(_assignmentData))]
    public void ParseStatement_CreatesParseTree(TestData data)
    {
        var source = data.Tokens.ToSourceState();

        var parsed = source.ParseStatement();

        Assert.IsEmpty(parsed.Source.Queue);
        Assert.AreEqual(data.ExpectedTree, parsed.Result);
    }

    [TestCaseSource(nameof(_assignmentData))]
    public void ParseBlock_CanContainAssignment(TestData data)
    {
        var tokensInBlock = data.Tokens.Prepend(new OpenBrace()).Append(new CloseBrace());
        var source = tokensInBlock.ToSourceState();
        var expectedTree = new Block(NodeList.From(new[] { (IBlockInner)data.ExpectedTree }), new Unit());

        var parsed = source.ParseBlock();

        Assert.IsEmpty(parsed.Source.Queue);
        Assert.AreEqual(expectedTree, parsed.Result);
    }
}