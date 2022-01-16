using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<FieldList> ParseFields()
    {
        var open = ParseToken<OpenBrace>();

        var fields = open.Source.ParseSeparated<FieldDeclaration, Comma, CloseBrace>(s => s.ParseFieldDeclaration());

        return fields.WithValue(new FieldList(NodeList.From(fields.Result)));
    }

    public IParseResult<FieldDeclaration> ParseFieldDeclaration()
    {
        var name = ParseToken<UppercaseIdentifier>();

        var colon = name.Source.ParseToken<Colon>();

        var definition = colon.Source.ParseExpression();

        return definition.WithValue(new FieldDeclaration(name.Result.Identifier, definition.Result));
    }
}