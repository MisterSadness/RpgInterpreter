var sourcePath = args[0];

if (args.Length == 1 && File.Exists(sourcePath))
{
    var sourceCodeString = File.ReadAllText(sourcePath).ReplaceLineEndings();

    var interpreter = new RpgInterpreter.RpgInterpreter();

    interpreter.InterpretString(sourceCodeString);
}
else
{
    Console.WriteLine("Pass a single parameter: path to the source file.");
}