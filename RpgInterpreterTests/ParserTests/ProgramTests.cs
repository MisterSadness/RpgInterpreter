using NUnit.Framework;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreterTests.Utils;
using AssignmentToken = RpgInterpreter.Lexer.Tokens.Assignment;
using IfToken = RpgInterpreter.Lexer.Tokens.If;

namespace RpgInterpreterTests.ParserTests;

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
                new CloseBrace(), new Semicolon(), new EndOfInput(), new LexingFinished()
            },
            AstFactory.Root(NodeList.From(new Statement[]
            {
                AstFactory.FunctionDeclaration(
                    "fight",
                    AstFactory.FunctionParameterList(NodeList.From(new[]
                    {
                        AstFactory.FunctionParameter("e1", "Entity"), AstFactory.FunctionParameter("e2", "Entity")
                    })),
                    "Unit",
                    AstFactory.Block(NodeList.From(new IBlockInner[]
                    {
                        AstFactory.Assignment(AstFactory.Variable("r1"), AstFactory.SubtractionExp(
                            AstFactory.DiceRoll(AstFactory.Dice(1, 100)),
                            AstFactory.DivisionExp(AstFactory.FieldReference(AstFactory.Variable("e1"), "Strength"),
                                AstFactory.Natural(10))
                        )),
                        AstFactory.Assignment(AstFactory.Variable("r2"), AstFactory.SubtractionExp(
                            AstFactory.DiceRoll(AstFactory.Dice(1, 100)),
                            AstFactory.DivisionExp(AstFactory.FieldReference(AstFactory.Variable("e2"), "Strength"),
                                AstFactory.Natural(10))
                        )),
                        AstFactory.Assignment(AstFactory.Variable("result"),
                            AstFactory.SubtractionExp(AstFactory.Variable("r1"), AstFactory.Variable("r2"))),
                        AstFactory.Assignment(AstFactory.Variable("loser"), AstFactory.If(
                            AstFactory.GreaterThanExp(AstFactory.Variable("result"), AstFactory.Natural(0)),
                            AstFactory.Variable("e2"),
                            AstFactory.Variable("e1"))),
                        AstFactory.Assignment(AstFactory.Variable("winner"), AstFactory.If(
                            AstFactory.GreaterThanExp(AstFactory.Variable("result"), AstFactory.Natural(0)),
                            AstFactory.Variable("e1"),
                            AstFactory.Variable("e2"))),
                        AstFactory.Assignment(
                            AstFactory.FieldReference(AstFactory.Variable("loser"), "Health"),
                            AstFactory.SubtractionExp(AstFactory.FieldReference(AstFactory.Variable("loser"), "Health"),
                                AstFactory.Variable("result"))
                        )
                    }), AstFactory.Unit()))
            }))
        )
    };

    [TestCaseSource(nameof(_programData))]
    public void ParseProgram_CreatesParseTree(TestData data)
    {
        var source = data.Tokens.ToSourceState();

        var parsed = source.ParseProgram();

        Assert.That(parsed.Source.PeekOrDefault(), Is.TypeOf<LexingFinished>());
        Assert.AreEqual(data.ExpectedTree, parsed.Result);
    }
}