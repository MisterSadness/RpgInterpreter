using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.CoolerParser.ParsingExceptions;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<Statement> ParseStatement()
    {
        return PeekOrDefault() switch
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