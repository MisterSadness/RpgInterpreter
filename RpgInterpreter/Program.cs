using System;
using System.IO;
using System.Linq;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

if (args.Length == 1 && File.Exists(args[0]))
{
    var lexer = new TrackingRpgLexer();
    using var source = new FileSource(args[0]);

    try
    {
        var tokens = lexer.Tokenize(source).ToArray();
        Console.WriteLine($"Successfully parsed, result token total: {tokens.Length}");
        Console.WriteLine("Result tokens (without whitespace):");
        foreach (var token in tokens.Select(t => t.Value).Where(t => t is not Whitespace)) Console.WriteLine(token);
    }
    catch (LexingException e)
    {
        Console.WriteLine($"Lexing failed with: {e}");
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