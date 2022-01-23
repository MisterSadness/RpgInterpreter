using NUnit.Framework;
using RpgInterpreter;

namespace RpgInterpreterTests.E2ETests;

public class FunctionTests
{
    private static IEnumerable<E2ETestCase> _printPrograms = new[]
    {
        new E2ETestCase(
            "fun factorial(i: Int) Int { if i == 0 then 1 else i * factorial(i-1); }; print(getString(factorial(5)));",
            "120")
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