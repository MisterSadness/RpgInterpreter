using System.Reflection;
using NUnit.Framework;
using RpgInterpreter.NonTerminals;
using RpgInterpreter.Parser;
using RpgInterpreter.Productions;
using RpgInterpreter.Tokens;

namespace RpgInterpreterTests.ParserTests
{
    internal class FollowTests
    {
        [TestCase]
        public void Test1()
        {
            var productionList = GetInstancesOfAllTypesInheriting<Production>();

            var generator = new ParsingTableGenerator(productionList);
            var _ = generator.CalculateFirst();
            var follows = generator.CalculateFollow();

            Assert.That(follows, Is.Not.Empty);
            Assert.That(follows.Values, Has.All.Not.Empty);
            Assert.That(follows.First().Value, Is.EquivalentTo(new Terminal[]
                { }));
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
                new SStart(), new SF(), new SParen(), new FA()
            };
            var generator = new ParsingTableGenerator(productionList);
            var _ = generator.CalculateFirst();
            var follows = generator.CalculateFollow();

            Assert.That(follows, Is.Not.Empty);
            Assert.That(follows.First().Value, Is.EquivalentTo(new Terminal[]
                { }));
        }

        private record S : NonTerminal;

        private record F : NonTerminal;

        private record A : Terminal<StringLiteral>;

        public record SStart() : Production<Syntax>(new Symbol[]
        {
            new S(), new Terminal<EndOfInput>()
        });

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
            var generator = new ParsingTableGenerator(productionList);
            var _ = generator.CalculateFirst();
            var follows = generator.CalculateFollow();

            Assert.That(follows, Is.Not.Empty);
        }

        private record Expr : NonTerminal;

        private record Expr1 : NonTerminal;

        private record Term : NonTerminal;

        private record Term1 : NonTerminal;

        private record Factor : NonTerminal;

        private record Start() : Production<Syntax>(new Symbol[] { new Expr(), new Terminal<EndOfInput>() });

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
}
