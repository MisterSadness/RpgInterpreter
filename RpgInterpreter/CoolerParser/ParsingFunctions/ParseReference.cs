using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser;
using RpgInterpreter.Utils;
using Base = RpgInterpreter.CoolerParser.Grammar.Base;
using This = RpgInterpreter.CoolerParser.Grammar.This;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<Reference> ParseSelfReference()
    {
        IParseResult<NameReference> obj = Queue.PeekOrDefault() switch
        {
            Lexer.Tokens.Base => ParseBase(),
            Lexer.Tokens.This => ParseThis(),
            _ => throw new ParsingException("Expected 'base' or 'this'.")
        };

        if (obj.Source.Queue.PeekOrDefault() is Access)
        {
            var parsedAccess = obj.Source.ParseToken<Access>();
            var parsedField = parsedAccess.Source.ParseToken<UppercaseIdentifier>();

            return parsedField.WithValue(
                new FieldReference(obj.Result, parsedField.Result.Identifier)
            );
        }

        return obj;
    }

    public IParseResult<Reference> ParseReference()
    {
        var parsedName = ParseToken<LowercaseIdentifier>();

        if (parsedName.Source.Queue.PeekOrDefault() is Access)
        {
            return parsedName.Source.ParseAccess(new Variable(parsedName.Result.Identifier));
        }

        if (parsedName.Source.Queue.PeekOrDefault() is OpenParen)
        {
            var functionName = parsedName.Result;

            var parsedOpenParen = parsedName.Source.ParseToken<OpenParen>();

            var arguments =
                parsedOpenParen.Source.ParseSeparated<Expression, Comma, CloseParen>(s => s.ParseExpression());

            return arguments.WithValue(new FunctionInvocation(functionName.Identifier, arguments.Result));
        }

        return parsedName.WithValue(new Variable(parsedName.Result.Identifier));
    }

    public IParseResult<FieldReference> ParseAccess(NameReference objectName)
    {
        var parsedAccess = ParseToken<Access>();
        var parsedField = parsedAccess.Source.ParseToken<UppercaseIdentifier>();

        return parsedField.WithValue(
            new FieldReference(objectName, parsedField.Result.Identifier)
        );
    }

    public IParseResult<Base> ParseBase()
    {
        var parsed = ParseToken<Lexer.Tokens.This>();

        return parsed.WithValue(new Base());
    }

    public IParseResult<This> ParseThis()
    {
        var parsed = ParseToken<Lexer.Tokens.This>();

        return parsed.WithValue(new This());
    }

    public IParseResult<FunctionInvocation> ParseFunctionInvocation()
    {
        var parsedName = ParseToken<LowercaseIdentifier>();
        var functionName = parsedName.Result;

        var parsedOpenParen = parsedName.Source.ParseToken<OpenParen>();

        var arguments = parsedOpenParen.Source.ParseSeparated<Expression, Comma, CloseParen>(s => s.ParseExpression());

        return arguments.WithValue(new FunctionInvocation(functionName.Identifier, arguments.Result));
    }

    public IParseResult<FunctionInvocationStatement> ParseFunctionInvocationStatement()
    {
        var parsed = ParseFunctionInvocation();
        return parsed.WithValue(new FunctionInvocationStatement(parsed.Result));
    }
}