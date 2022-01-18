using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Parser;

if (args.Length == 1 && File.Exists(args[0]))
{
    var lexer = new TrackingRpgLexer();
    using var source = new FileSource(args[0]);

    var parser = new CoolerParser(lexer.Tokenize(source));

    var handler = new RpgInterpreterExceptionHandler();

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