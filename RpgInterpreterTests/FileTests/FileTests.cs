using NUnit.Framework;
using RpgInterpreter;

namespace RpgInterpreterTests.FileTests;

public class FileTests
{
    [Test]
    public void TestsFromFile()
    {
        var inputs = Directory.GetFiles(Path.Combine(TestContext.CurrentContext.WorkDirectory, "FileTests", "Input"))
            .Where(s => s.EndsWith(".in")).OrderBy(s => s);
        var outputs = Directory.GetFiles(Path.Combine(TestContext.CurrentContext.WorkDirectory, "FileTests", "Output"))
            .Where(s => s.EndsWith(".out")).OrderBy(s => s);

        var testCases = inputs.Zip(outputs).ToList();

        var totalTests = testCases.Count;
        var currentTest = 0;

        Console.WriteLine($"Discovered {totalTests} test cases.");

        foreach (var (inFile, outFile) in testCases)
        {
            Console.WriteLine($"({++currentTest}/{totalTests}) {Path.GetFileName(inFile)[..^3]} Test:");
            var inString = File.ReadAllText(inFile).ReplaceLineEndings();
            var outString = File.ReadAllText(outFile).ReplaceLineEndings();

            var output = new DoubleOutput();
            var interpreter = new RpgInterpreter.RpgInterpreter(output);

            interpreter.InterpretString(inString);

            var result = output.Read().ReplaceLineEndings();

            Assert.That(result, Is.EqualTo(outString));

            Console.WriteLine("Passed!");
            Console.WriteLine();
        }
    }
}