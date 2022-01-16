using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<FunctionDeclaration> ParseFunctionDeclaration()
    {
        var parsedFun = ParseToken<Fun>();
        var parsedFunctionName = parsedFun.Source.ParseToken<LowercaseIdentifier>();
        var parsedFunctionParameterList = parsedFunctionName.Source.ParseFunctionParameterList();
        var parsedReturnType = parsedFunctionParameterList.Source.ParseToken<UppercaseIdentifier>();
        var parsedBody = parsedReturnType.Source.ParseBlock();

        return parsedBody.WithValue(new FunctionDeclaration(
            parsedFunctionName.Result.Identifier,
            parsedFunctionParameterList.Result,
            parsedReturnType.Result.Identifier,
            parsedBody.Result
        ));
    }

    public IParseResult<FunctionParameterList> ParseFunctionParameterList()
    {
        var parsedOpen = ParseToken<OpenParen>();
        var parsedParams = parsedOpen.Source.ParseSeparated<FunctionParameter, Comma, CloseParen>(
            s => s.ParseFunctionParameter()
        );

        return parsedParams.WithValue(new FunctionParameterList(NodeList.From(parsedParams.Result)));
    }

    public IParseResult<FunctionParameter> ParseFunctionParameter()
    {
        var parsedName = ParseToken<LowercaseIdentifier>();
        var parsedColon = parsedName.Source.ParseToken<Colon>();
        var parsedType = parsedColon.Source.ParseToken<UppercaseIdentifier>();

        return parsedType.WithValue(new FunctionParameter(parsedName.Result.Identifier, parsedType.Result.Identifier));
    }
}