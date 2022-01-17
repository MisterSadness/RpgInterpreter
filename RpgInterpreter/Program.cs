using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Parser;

if (args.Length == 1 && File.Exists(args[0]))
{
    var lexer = new TrackingRpgLexer();
    using var source = new FileSource(args[0]);

    try
    {
        //var tokens = lexer.Tokenize(source).ToArray();
        //Console.WriteLine($"Successfully lexed, result token total: {tokens.Length}");
        //Console.WriteLine("Result tokens (without whitespace):");
        //foreach (var token in tokens.Select(t => t.Value).Where(t => t is not Whitespace)) Console.WriteLine(token);

        var parser = new Parser(source);
        var parsingResult = parser.Parse();
        Console.WriteLine(parsingResult);
    }
    catch (LexingException e)
    {
        Console.WriteLine($"Lexing failed with: {e}");
    }
    catch (ObsoleteUnexpectedTokenException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (ObsoleteExpectedTokenNotFoundException e)
    {
        Console.WriteLine(e.Message);
    }
    catch (Exception)
    {
        Console.WriteLine("Unexpected exception has occurred.");
        throw;
    }
}
else
{
    Console.WriteLine("Pass a single parameter: path to the source file.");
}