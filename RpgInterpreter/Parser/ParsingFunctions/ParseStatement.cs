using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreter.Parser.ParsingExceptions;

namespace RpgInterpreter.Parser.ParsingFunctions;

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