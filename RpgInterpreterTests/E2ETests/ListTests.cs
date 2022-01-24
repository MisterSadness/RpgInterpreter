using NUnit.Framework;
using RpgInterpreter.Output;

namespace RpgInterpreterTests.E2ETests;

public class ListTests
{
    private static IEnumerable<E2ETestCase> _correctCases = new[]
    {
        new E2ETestCase("Entity { Stuff: [] }; set x = Entity; print(getString(x));",
            "Entity {\n  Stuff: []\n}\n".ReplaceLineEndings()),
        new E2ETestCase("Entity { Stuff: [] }; " +
                        "trait Strong for Entity { Stuff: base.Stuff ++ [3] }; " +
                        "StrongEntity extends Entity with Strong {};" +
                        "set x = StrongEntity; " +
                        "print(getString(x));",
            "StrongEntity {\n  Stuff: [3]\n}\n".ReplaceLineEndings()),
        new E2ETestCase("Entity { Stuff: [] }; " +
                        "trait Strong for Entity { Stuff: base.Stuff ++ [3] }; " +
                        "StrongEntity extends Entity with Strong {};" +
                        "set x = StrongEntity;" +
                        "set x.Stuff = [];" +
                        "print(getString(x));",
            "StrongEntity {\n  Stuff: []\n}\n".ReplaceLineEndings()),
        new E2ETestCase("Entity { Stuff: [] }; " +
                        "trait Strong for Entity { Stuff: base.Stuff ++ [3] }; " +
                        "StrongEntity extends Entity with Strong {};" +
                        "set x = StrongEntity;" +
                        "set x.Stuff = [];" +
                        "set x.Stuff = [false];" +
                        "print(getString(x));",
            ("Semantic exception occurred: Type inference failed: Expected List[Int], but got List[Bool]." +
             "\nAt: line 0, column 157" +
             "\nEntity { Stuff: [] }; trait Strong for Entity { Stuff: base.Stuff ++ [3] }; StrongEntity extends Entity with Strong {};set x = StrongEntity;set x.Stuff = [];set x.Stuff = [false];print(getString(x));" +
             "\n                                                                                                                                                             ^^^^^^^^^^^^^^^^^^^^^^\n\n"
            ).ReplaceLineEndings()),
        new E2ETestCase("Entity { Stuff: [] };" +
                        "trait Strong for Entity { Stuff: base.Stuff ++ [\"Test\"]};" +
                        "set x = Entity with Strong;" +
                        "print(getString(x));",
            "anonymous {\n  Stuff: [Test]\n}\n".ReplaceLineEndings()),
        new E2ETestCase("set x = []; set x = [1,2,3];", ""),
        new E2ETestCase("set x = []; set x = [1,2,3]; set x = []; set x = [false];",
            ("Semantic exception occurred: Type inference failed: Expected List[Int], but got List[Bool]." +
             "\nAt: line 0, column 40" +
             "\nset x = []; set x = [1,2,3]; set x = []; set x = [false];" +
             "\n                                        ^^^^^^^^^^^^^^^^^\n\n").ReplaceLineEndings()),
        new E2ETestCase("set x = []; set y = x ++ [1,2,3];", "")
    };

    [TestCaseSource(nameof(_correctCases))]
    public void ListTest(E2ETestCase data)
    {
        var output = new DoubleOutput();
        var interpreter = new RpgInterpreter.RpgInterpreter(output);

        interpreter.InterpretString(data.Input);

        Assert.That(output.Read(), Is.EqualTo(data.Output));
    }
}