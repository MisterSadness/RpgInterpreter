using NUnit.Framework;
using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreterTests.CoolerParserTests.Extensions;
using Assignment = RpgInterpreter.CoolerParser.Grammar.Assignment;
using AssignmentToken = RpgInterpreter.Lexer.Tokens.Assignment;
using If = RpgInterpreter.CoolerParser.Grammar.If;
using IfToken = RpgInterpreter.Lexer.Tokens.If;

namespace RpgInterpreterTests.CoolerParserTests;

internal class ProgramTests
{
    public record struct TestData(IEnumerable<Token> Tokens, object ExpectedTree);

    private static IEnumerable<TestData> _programData = new TestData[]
    {
        /*
         * fun fight(e1: Entity, e2: Entity) Unit {
               set r1 = 1d100() - e1.Strength / 10;
               set r2 = 1d100() - e2.Strength / 10;
               set result = r1 - r2;
               set loser = if (result > 0)
                    then e2
                    else e1;
               set winner = if (result > 0)
                    then e1
                    else e2;
               set loser.Health = loser.Health - result;
           };
         */
        new(new Token[]
            {
                new Fun(), new LowercaseIdentifier("fight"), new OpenParen(), new LowercaseIdentifier("e1"),
                new Colon(), new UppercaseIdentifier("Entity"),
                new Comma(), new LowercaseIdentifier("e2"), new Colon(), new UppercaseIdentifier("Entity"),
                new CloseParen(), new UppercaseIdentifier("Unit"), new OpenBrace(),
                new Set(), new LowercaseIdentifier("r1"), new AssignmentToken(), new DiceLiteral(1, 100),
                new OpenParen(),
                new CloseParen(),
                new Minus(), new LowercaseIdentifier("e1"), new Access(), new UppercaseIdentifier("Strength"),
                new Division(), new NaturalLiteral(10), new Semicolon(),
                new Set(), new LowercaseIdentifier("r2"), new AssignmentToken(), new DiceLiteral(1, 100),
                new OpenParen(),
                new CloseParen(),
                new Minus(), new LowercaseIdentifier("e2"), new Access(), new UppercaseIdentifier("Strength"),
                new Division(), new NaturalLiteral(10), new Semicolon(),
                new Set(), new LowercaseIdentifier("result"), new AssignmentToken(), new LowercaseIdentifier("r1"),
                new Minus(), new LowercaseIdentifier("r2"), new Semicolon(),
                new Set(), new LowercaseIdentifier("loser"), new AssignmentToken(), new IfToken(), new OpenParen(),
                new LowercaseIdentifier("result"), new Greater(), new NaturalLiteral(0), new CloseParen(),
                new Then(), new LowercaseIdentifier("e2"),
                new Else(), new LowercaseIdentifier("e1"), new Semicolon(),
                new Set(), new LowercaseIdentifier("winner"), new AssignmentToken(), new IfToken(), new OpenParen(),
                new LowercaseIdentifier("result"), new Greater(), new NaturalLiteral(0), new CloseParen(),
                new Then(), new LowercaseIdentifier("e1"),
                new Else(), new LowercaseIdentifier("e2"), new Semicolon(),
                new Set(), new LowercaseIdentifier("loser"), new Access(), new UppercaseIdentifier("Health"),
                new AssignmentToken(),
                new LowercaseIdentifier("loser"), new Access(), new UppercaseIdentifier("Health"), new Minus(),
                new LowercaseIdentifier("result"), new Semicolon(),
                new CloseBrace(), new Semicolon(), new EndOfInput()
            },
            new Root(NodeList.From(new Statement[]
            {
                new FunctionDeclaration(
                    "fight",
                    new FunctionParameterList(NodeList.From(new FunctionParameter[]
                        { new("e1", "Entity"), new("e2", "Entity") })),
                    "Unit",
                    new Block(NodeList.From(new IBlockInner[]
                    {
                        new Assignment(new Variable("r1"), new SubtractionExp(
                            new DiceRoll(new Dice(1, 100)),
                            new DivisionExp(new FieldReference(new Variable("e1"), "Strength"), new Natural(10))
                        )),
                        new Assignment(new Variable("r2"), new SubtractionExp(
                            new DiceRoll(new Dice(1, 100)),
                            new DivisionExp(new FieldReference(new Variable("e2"), "Strength"), new Natural(10))
                        )),
                        new Assignment(new Variable("result"),
                            new SubtractionExp(new Variable("r1"), new Variable("r2"))),
                        new Assignment(new Variable("loser"), new If(
                            new GreaterThanExp(new Variable("result"), new Natural(0)),
                            new Variable("e2"),
                            new Variable("e1"))),
                        new Assignment(new Variable("winner"), new If(
                            new GreaterThanExp(new Variable("result"), new Natural(0)),
                            new Variable("e1"),
                            new Variable("e2"))),
                        new Assignment(
                            new FieldReference(new Variable("loser"), "Health"),
                            new SubtractionExp(new FieldReference(new Variable("loser"), "Health"),
                                new Variable("result"))
                        )
                    }), new Unit()))
            }))
        )
    };

    [TestCaseSource(nameof(_programData))]
    public void ParseProgram_CreatesParseTree(TestData data)
    {
        var source = data.Tokens.ToSourceState();

        var parsed = source.ParseProgram();

        Assert.IsEmpty(parsed.Source.Queue);
        Assert.AreEqual(data.ExpectedTree, parsed.Result);
    }
}