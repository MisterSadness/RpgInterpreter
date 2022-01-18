using NUnit.Framework;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreterTests.ParserTests.Utils;

namespace RpgInterpreterTests.ParserTests;

internal class BinaryOperationTests
{
    public record TestData(IEnumerable<Token> Input, Expression Expected);

    private static IEnumerable<TestData> _precedenceData = new[]
    {
        new TestData(new Token[]
            {
                new NaturalLiteral(1), new Addition(), new NaturalLiteral(2), new Multiplication(),
                new NaturalLiteral(3), new LexingFinished()
            },
            AstFactory.AdditionExp(AstFactory.Natural(1),
                AstFactory.MultiplicationExp(AstFactory.Natural(2), AstFactory.Natural(3)))
        ),
        new TestData(new Token[]
            {
                new NaturalLiteral(1), new Multiplication(), new NaturalLiteral(2), new Addition(),
                new NaturalLiteral(3), new LexingFinished()
            },
            AstFactory.AdditionExp(AstFactory.MultiplicationExp(AstFactory.Natural(1), AstFactory.Natural(2)),
                AstFactory.Natural(3))
        ),
        new TestData(new Token[]
            {
                new NaturalLiteral(1), new Multiplication(), new Minus(), new NaturalLiteral(2), new Addition(),
                new NaturalLiteral(3), new LexingFinished()
            },
            AstFactory.AdditionExp(
                AstFactory.MultiplicationExp(AstFactory.Natural(1), AstFactory.UnaryMinus(AstFactory.Natural(2))),
                AstFactory.Natural(3))
        )
    };

    [TestCaseSource(nameof(_precedenceData))]
    public void Precedence(TestData data)
    {
        var sourceState = data.Input.ToSourceState();

        var result = sourceState.ParseExpression();

        Assert.That(result.Source.PeekOrDefault(), Is.TypeOf<LexingFinished>());
        Assert.That(result.Result, Is.EqualTo(data.Expected));
    }

    private static IEnumerable<IEnumerable<Token>> _additionData = new[]
    {
        new Token[] { new NaturalLiteral(3), new Addition(), new NaturalLiteral(4), new LexingFinished() }
    };

    [TestCaseSource(nameof(_additionData))]
    public void Addition(IEnumerable<Token> source)
    {
        var sourceState = source.ToSourceState();

        var result = sourceState.ParseExpression();

        Assert.That(result.Result, Is.TypeOf<AdditionExp>());
        Assert.That(result.Source.PeekOrDefault(), Is.TypeOf<LexingFinished>());
    }
}