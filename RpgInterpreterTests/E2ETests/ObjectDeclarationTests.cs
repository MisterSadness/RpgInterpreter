using NUnit.Framework;
using RpgInterpreter.Output;

namespace RpgInterpreterTests.E2ETests;

public class ObjectDeclarationTests
{
    private static IEnumerable<E2ETestCase> _correctCases = new[]
    {
        new E2ETestCase("Entity { Strength: 10 }; set x = Entity; print(getString(x));",
            "Entity {\n  Strength: 10\n}\n".ReplaceLineEndings()),
        new E2ETestCase("Entity { Strength: 10 }; " +
                        "trait Strong for Entity { Strength: base.Strength + 10 }; " +
                        "StrongEntity extends Entity with Strong {};" +
                        "set x = StrongEntity; " +
                        "print(getString(x));",
            "StrongEntity {\n  Strength: 20\n}\n".ReplaceLineEndings()),
        new E2ETestCase("Entity { Strength: 10 };" +
                        "trait Strong for Entity { Strength: base.Strength + 10 };" +
                        "set x = Entity with Strong;" +
                        "print(getString(x));",
            "anonymous {\n  Strength: 20\n}\n".ReplaceLineEndings())
    };

    [TestCaseSource(nameof(_correctCases))]
    public void ObjectDeclarationTest(E2ETestCase data)
    {
        var output = new StringOutput();
        var interpreter = new RpgInterpreter.RpgInterpreter(output);

        interpreter.InterpretString(data.Input);

        Assert.That(output.Read(), Is.EqualTo(data.Output));
    }
}