using NUnit.Framework;
using RpgInterpreter.Output;

namespace RpgInterpreterTests.E2ETests;

public class AssignmentTests
{
    private static readonly IEnumerable<E2ETestCase> _programs = new[]
    {
        new E2ETestCase("set x = \"abc\"; set y = 7; print(x ++ getString(y));", "abc7"),
        new E2ETestCase("set x = 3; set y = 7; print(getString(x));", "3"),
        new E2ETestCase("set x = 3; set x = 7; print(getString(x));", "7"),
        new E2ETestCase("set x = \"abc\"; print(x);", "abc")
    };

    [TestCaseSource(nameof(_programs))]
    public void AssignmentTest(E2ETestCase data)
    {
        var output = new StringOutput();
        var interpreter = new RpgInterpreter.RpgInterpreter(output);

        interpreter.InterpretString(data.Input);

        Assert.That(output.Read(), Is.EqualTo(data.Output));
    }
}