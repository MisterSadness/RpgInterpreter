using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreter.Parser.ParsingExceptions;
using Base = RpgInterpreter.Parser.Grammar.Base;
using This = RpgInterpreter.Parser.Grammar.This;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<Reference> ParseSelfReference()
    {
        var start = CurrentPosition;
        IParseResult<NameReference> obj = PeekOrDefault() switch
        {
            Lexer.Tokens.Base => ParseBase(),
            Lexer.Tokens.This => ParseThis(),
            _ => throw new ParsingException("Expected 'base' or 'this'.")
        };

        if (obj.Source.PeekOrDefault() is Access)
        {
            var parsedAccess = obj.Source.ParseToken<Access>();
            var parsedField = parsedAccess.Source.ParseToken<UppercaseIdentifier>();
            var end = obj.Source.CurrentPosition;

            return parsedField.WithValue(
                new FieldReference(obj.Result, parsedField.Result.Identifier, start, end)
            );
        }

        return obj;
    }

    public IParseResult<Reference> ParseReference()
    {
        var start = CurrentPosition;
        var parsedName = ParseToken<LowercaseIdentifier>();
        var end = parsedName.Source.CurrentPosition;

        if (parsedName.Source.PeekOrDefault() is Access)
        {
            return parsedName.Source.ParseAccess(new VariableExp(parsedName.Result.Identifier, start, end));
        }

        if (parsedName.Source.PeekOrDefault() is OpenParen)
        {
            var functionName = parsedName.Result;

            var parsedOpenParen = parsedName.Source.ParseToken<OpenParen>();

            var arguments =
                parsedOpenParen.Source.ParseSeparated<Expression, Comma, CloseParen>(s => s.ParseExpression());
            end = arguments.Source.CurrentPosition;

            return arguments.WithValue(new FunctionInvocation(functionName.Identifier, arguments.Result, start,
                end));
        }

        end = parsedName.Source.CurrentPosition;
        return parsedName.WithValue(new VariableExp(parsedName.Result.Identifier, start, end));
    }

    public IParseResult<FieldReference> ParseAccess(NameReference objectName)
    {
        var start = objectName.Start;
        var parsedAccess = ParseToken<Access>();
        var parsedField = parsedAccess.Source.ParseToken<UppercaseIdentifier>();
        var end = parsedField.Source.CurrentPosition;

        return parsedField.WithValue(
            new FieldReference(objectName, parsedField.Result.Identifier, start, end)
        );
    }

    public IParseResult<Base> ParseBase()
    {
        var start = CurrentPosition;
        var parsed = ParseToken<Lexer.Tokens.Base>();
        var end = parsed.Source.CurrentPosition;

        return parsed.WithValue(new Base(start, end));
    }

    public IParseResult<This> ParseThis()
    {
        var start = CurrentPosition;
        var parsed = ParseToken<Lexer.Tokens.This>();
        var end = parsed.Source.CurrentPosition;

        return parsed.WithValue(new This(start, end));
    }
}