using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser;
using RpgInterpreter.Utils;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<Statement> ParseStatement()
    {
        return Queue.PeekOrDefault() switch
        {
            UppercaseIdentifier => ParseObjectDeclaration(),
            Trait => ParseTraitDeclaration(),
            Set => ParseAssignment(),
            LowercaseIdentifier => ParseFunctionInvocationStatement(),
            Fun => ParseFunctionDeclaration(),
            _ => throw new ParsingException("Expected statement.")
        };
    }
}