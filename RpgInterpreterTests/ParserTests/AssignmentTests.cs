using NUnit.Framework;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.ParsingExceptions;
using RpgInterpreterTests.ParserTests.Utils;

namespace RpgInterpreterTests.ParserTests;

internal class AssignmentTests
{
    public record struct TestData(IEnumerable<Token> Tokens, object ExpectedTree);

    private static IEnumerable<TestData> _assignmentData = new TestData[]
    {
        new(
            new Token[]
            {
                new Set(), new LowercaseIdentifier("x"), new Assignment(), new NaturalLiteral(42), new LexingFinished()
            },
            AstFactory.Assignment(AstFactory.Variable("x"),
                AstFactory.Natural(42))
        )
    };

    [Test]
    public void WithoutSetKeyword_Fails()
    {
        var tokens = new Token[]
            { new LowercaseIdentifier("x"), new Assignment(), new NaturalLiteral(42), new LexingFinished() };
        var source = tokens.ToSourceState();

        Assert.That(() => source.ParseAssignment(), Throws.InstanceOf<ParsingException>());
        //Assert.Throws<ExpectedTokenNotFoundException<Set>>(() => source.ParseAssignment());
    }

    [TestCaseSource(nameof(_assignmentData))]
    public void ParseAssignment_CreatesParseTree(TestData data)
    {
        var source = data.Tokens.ToSourceState();

        var parsed = source.ParseAssignment();

        Assert.That(parsed.Source.PeekOrDefault(), Is.TypeOf<LexingFinished>());
        Assert.AreEqual(data.ExpectedTree, parsed.Result);
    }

    [TestCaseSource(nameof(_assignmentData))]
    public void ParseStatement_CreatesParseTree(TestData data)
    {
        var source = data.Tokens.ToSourceState();

        var parsed = source.ParseStatement();

        Assert.That(parsed.Source.PeekOrDefault(), Is.TypeOf<LexingFinished>());
        Assert.AreEqual(data.ExpectedTree, parsed.Result);
    }
}