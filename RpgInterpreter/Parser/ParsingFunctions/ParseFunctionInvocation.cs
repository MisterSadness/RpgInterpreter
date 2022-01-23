using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<FunctionInvocation> ParseFunctionInvocation()
    {
        var start = CurrentPosition;
        var parsedName = ParseToken<LowercaseIdentifier>();
        var functionName = parsedName.Result;
        var parsedOpenParen = parsedName.Source.ParseToken<OpenParen>();
        var arguments = parsedOpenParen.Source.ParseSeparated<Expression, Comma, CloseParen>(s => s.ParseExpression());
        var end = arguments.Source.CurrentPosition;

        return arguments.WithValue(new FunctionInvocation(functionName.Identifier, arguments.Result, start, end));
    }

    public IParseResult<FunctionInvocationStatement> ParseFunctionInvocationStatement()
    {
        var start = CurrentPosition;
        var parsed = ParseFunctionInvocation();
        var end = parsed.Source.CurrentPosition;
        return parsed.WithValue(new FunctionInvocationStatement(parsed.Result, start, end));
    }
}