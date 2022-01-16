using System.Reflection;
using NUnit.Framework;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.NonTerminals;
using RpgInterpreter.Parser;
using RpgInterpreter.Productions;
using If = RpgInterpreter.Lexer.Tokens.If;

namespace RpgInterpreterTests.ParserTests;

[Ignore("Ignore old parser tests")]
internal class FirstTests
{
    [TestCase]
    public void Test1()
    {
        var productionList = GetInstancesOfAllTypesInheriting<Production>();

        var firsts = new ParsingTableGenerator(productionList).CalculateFirst();

        Assert.That(firsts, Is.Not.Empty);
        Assert.That(firsts.Values, Has.All.Not.Empty);
        Assert.That(firsts[new RpgProgram()], Is.EquivalentTo(new Terminal[]
        {
            new Epsilon(),
            new Terminal<UppercaseIdentifier>(),
            new Terminal<Trait>(),
            new Terminal<LowercaseIdentifier>(),
            new Terminal<This>(),
            new Terminal<Base>(),
            new Terminal<Fun>(),
            new Terminal<Minus>(),
            new Terminal<OpenParen>(),
            new Terminal<DiceLiteral>(),
            new Terminal<NaturalLiteral>(),
            new Terminal<BooleanLiteral>(),
            new Terminal<StringLiteral>(),
            new Terminal<OpenBrace>(),
            new Terminal<If>(),
            new Terminal<OpenBracket>()
        }));
    }

    private IEnumerable<T> GetInstancesOfAllTypesInheriting<T>()
    {
        var types = typeof(T).Assembly.GetTypes().Where(t =>
            t.IsAssignableTo(typeof(T)) && !t.IsAbstract && !t.ContainsGenericParameters);
        var ctors = types.Select(t =>
            t.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                Array.Empty<Type>()));

        return ctors.Select(c => (T)c.Invoke(null));
    }

    [TestCase]
    public void Test2()
    {
        var productionList = new List<Production>
        {
            new SF(), new SParen(), new FA()
        };
        var firsts = new ParsingTableGenerator(productionList).CalculateFirst();

        Assert.That(firsts, Is.Not.Empty);
    }

    [TestCase]
    public void Test3()
    {
        var productionList = new List<Production>
        {
            new Start(), new TermExpr1(), new Plus(), new MinusEx(), new ExprEps(), new FactorTerm1(), new Mul(),
            new Div(),
            new TermEps(),
            new Parens(), new Number()
        };
        var firsts = new ParsingTableGenerator(productionList).CalculateFirst();

        Assert.That(firsts, Is.Not.Empty);
    }

    private record S : NonTerminal;

    private record F : NonTerminal;

    private record A : Terminal<StringLiteral>;

    private record SF() : Production<S>(new Symbol[] { new F() });

    private record SParen() : Production<S>(new Symbol[]
    {
        new Terminal<OpenParen>(),
        new S(),
        new Terminal<Addition>(),
        new F(),
        new Terminal<CloseParen>()
    });

    private record FA() : Production<F>(new Symbol[] { new A() });

    private record Expr : NonTerminal;

    private record Expr1 : NonTerminal;

    private record Term : NonTerminal;

    private record Term1 : NonTerminal;

    private record Factor : NonTerminal;

    private record Start() : Production<S>(new Symbol[] { new Expr(), new Terminal<EndOfInput>() });

    private record TermExpr1() : Production<Expr>(new Symbol[] { new Term(), new Expr1() });

    private record Plus() : Production<Expr1>(new Symbol[] { new Terminal<Addition>(), new Term(), new Expr1() });

    private record MinusEx() : Production<Expr1>(new Symbol[] { new Terminal<Minus>(), new Term(), new Expr1() });

    private record ExprEps() : Production<Expr1>(new Symbol[] { new Epsilon() });

    private record FactorTerm1() : Production<Term>(new Symbol[] { new Factor(), new Term1() });

    private record Mul() : Production<Term1>(new Symbol[]
        { new Terminal<Multiplication>(), new Factor(), new Term1() });

    private record Div() : Production<Term1>(new Symbol[] { new Terminal<Division>(), new Factor(), new Term1() });

    private record TermEps() : Production<Term1>(new Symbol[] { new Epsilon() });

    private record Parens() : Production<Factor>(new Symbol[]
        { new Terminal<OpenParen>(), new Expr(), new Terminal<CloseParen>() });

    private record Number() : Production<Factor>(new Symbol[] { new Terminal<NaturalLiteral>() });
}