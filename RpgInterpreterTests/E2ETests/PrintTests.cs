using NUnit.Framework;
using RpgInterpreter.Output;

namespace RpgInterpreterTests.E2ETests;

public class PrintTests
{
    private static IEnumerable<E2ETestCase> _printPrograms = new[]
    {
        new E2ETestCase("print(\"Hello World!\");", "Hello World!"),
        new E2ETestCase("print(getString(7));", "7")
    };

    [TestCaseSource(nameof(_printPrograms))]
    public void PrintTest(E2ETestCase data)
    {
        var output = new StringOutput();
        var interpreter = new RpgInterpreter.RpgInterpreter(output);

        interpreter.InterpretString(data.Input);

        Assert.That(output.Read(), Is.EqualTo(data.Output));
    }
}