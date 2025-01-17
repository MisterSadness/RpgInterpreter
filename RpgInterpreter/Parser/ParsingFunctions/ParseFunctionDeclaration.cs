﻿using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<FunctionDeclaration> ParseFunctionDeclaration()
    {
        var start = CurrentPosition;
        var parsedFun = ParseToken<Fun>();
        var parsedFunctionName = parsedFun.Source.ParseToken<LowercaseIdentifier>();
        var parsedFunctionParameterList = parsedFunctionName.Source.ParseFunctionParameterList();
        var parsedReturnType = parsedFunctionParameterList.Source.ParseToken<UppercaseIdentifier>();
        var parsedBody = parsedReturnType.Source.ParseBlock();
        var end = parsedBody.Source.CurrentPosition;

        return parsedBody.WithValue(new FunctionDeclaration(
            parsedFunctionName.Result.Identifier,
            parsedFunctionParameterList.Result,
            parsedReturnType.Result.Identifier,
            parsedBody.Result,
            start, end
        ));
    }

    public IParseResult<FunctionParameterList> ParseFunctionParameterList()
    {
        var start = CurrentPosition;
        var parsedOpen = ParseToken<OpenParen>();
        var parsedParams = parsedOpen.Source.ParseSeparated<FunctionParameterDeclaration, Comma, CloseParen>(
            s => s.ParseFunctionParameter()
        );
        var end = parsedParams.Source.CurrentPosition;

        return parsedParams.WithValue(new FunctionParameterList(NodeList.From(parsedParams.Result), start, end));
    }

    public IParseResult<FunctionParameterDeclaration> ParseFunctionParameter()
    {
        var start = CurrentPosition;
        var parsedName = ParseToken<LowercaseIdentifier>();
        var parsedColon = parsedName.Source.ParseToken<Colon>();
        var parsedType = parsedColon.Source.ParseToken<UppercaseIdentifier>();
        var end = parsedType.Source.CurrentPosition;

        return parsedType.WithValue(new FunctionParameterDeclaration(parsedName.Result.Identifier,
            parsedType.Result.Identifier,
            start, end));
    }
}