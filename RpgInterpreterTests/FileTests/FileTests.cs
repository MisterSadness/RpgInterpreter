using NUnit.Framework;
using RpgInterpreter.Output;

namespace RpgInterpreterTests.FileTests;

public class FileTests
{
    public record TestFilePaths(string In, string Out)
    {
        public override string ToString() => $"In = {Path.GetFileName(In)}, Out = {Path.GetFileName(Out)}";
    }

    public static TestFilePaths[] GetFiles()
    {
        var inputs = Directory.GetFiles(Path.Combine(TestContext.CurrentContext.WorkDirectory, "FileTests", "Input"))
            .Where(s => s.EndsWith(".in")).OrderBy(s => s);
        var outputs = Directory.GetFiles(Path.Combine(TestContext.CurrentContext.WorkDirectory, "FileTests", "Output"))
            .Where(s => s.EndsWith(".out")).OrderBy(s => s);

        return inputs.Zip(outputs, (input, output) => new TestFilePaths(input, output)).ToArray();
    }

    [TestCaseSource(nameof(GetFiles))]
    public void TestsFromFile(TestFilePaths paths)
    {
        var inString = File.ReadAllText(paths.In).ReplaceLineEndings();
        var outString = File.ReadAllText(paths.Out).ReplaceLineEndings().Trim();

        var output = new DoubleOutput();
        var interpreter = new RpgInterpreter.RpgInterpreter(output);

        interpreter.InterpretString(inString);

        var result = output.Read().ReplaceLineEndings().Trim();

        Assert.That(result, Is.EqualTo(outString));
    }
}