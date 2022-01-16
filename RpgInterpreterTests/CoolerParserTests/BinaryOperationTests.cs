using System.Collections.Immutable;
using NUnit.Framework;
using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.CoolerParser.ParsingFunctions;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreterTests.CoolerParserTests.Extensions;

namespace RpgInterpreterTests.CoolerParserTests
{
    internal class BinaryOperationTests
    {
        public record TestData(IEnumerable<Token> Input, Expression Expected);

        private static IEnumerable<TestData> _precedenceData = new[]
        {
            new TestData(new Token[]
                {
                    new NaturalLiteral(1), new Addition(), new NaturalLiteral(2), new Multiplication(),
                    new NaturalLiteral(3)
                },
                new AdditionExp(new Natural(1), new MultiplicationExp(new Natural(2), new Natural(3)))
            ),
            new TestData(new Token[]
                {
                    new NaturalLiteral(1), new Multiplication(), new NaturalLiteral(2), new Addition(),
                    new NaturalLiteral(3)
                },
                new AdditionExp(new MultiplicationExp(new Natural(1), new Natural(2)), new Natural(3))
            ),
            new TestData(new Token[]
                {
                    new NaturalLiteral(1), new Multiplication(), new Minus(), new NaturalLiteral(2), new Addition(),
                    new NaturalLiteral(3)
                },
                new AdditionExp(new MultiplicationExp(new Natural(1), new UnaryMinus(new Natural(2))), new Natural(3))
            )
        };

        [TestCaseSource(nameof(_precedenceData))]
        public void Precedence(TestData data)
        {
            var sourceState = data.Input.ToSourceState();

            var result = sourceState.ParseExpression();

            Assert.That(result.Source.Queue.IsEmpty);
            Assert.That(result.Result, Is.EqualTo(data.Expected));
        }

        private static IEnumerable<IEnumerable<Token>> _additionData = new[]
        {
            new Token[] { new NaturalLiteral(3), new Addition(), new NaturalLiteral(4) }
        };

        [TestCaseSource(nameof(_additionData))]
        public void Addition(IEnumerable<Token> source)
        {
            var sourceState = new SourceState(ImmutableQueue.CreateRange(source));

            var result = sourceState.ParseExpression();

            Assert.That(result.Result, Is.TypeOf<AdditionExp>());
            Assert.That(result.Source.Queue.IsEmpty);
        }
    }
}
