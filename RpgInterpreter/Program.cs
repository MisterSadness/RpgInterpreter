using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Parser;

var sourcePath = args[0];

if (args.Length == 1 && File.Exists(sourcePath))
{
    var sourceCodeString = File.ReadAllText(sourcePath);
    var errorLocator = new ErrorAreaPrinter(sourceCodeString);

    var lexer = new TrackingRpgLexer();
    var stringSource = new StringSource(sourceCodeString);
    using var source = new FileSource(sourcePath);

    var parser = new CoolerParser(lexer.Tokenize(stringSource));

    var handler = new RpgInterpreterExceptionHandler(errorLocator);

    handler.RunAndHandle(() =>
    {
        var root = parser.Parse();

        Console.WriteLine(root.ToString());
    });
}
else
{
    Console.WriteLine("Pass a single parameter: path to the source file.");
}