using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<FieldList> ParseFields()
    {
        var start = CurrentPosition;
        var open = ParseToken<OpenBrace>();
        var fields = open.Source.ParseSeparated<FieldDeclaration, Comma, CloseBrace>(s => s.ParseFieldDeclaration());
        var end = fields.Source.CurrentPosition;

        return fields.WithValue(new FieldList(NodeList.From(fields.Result), start, end));
    }

    public IParseResult<FieldDeclaration> ParseFieldDeclaration()
    {
        var start = CurrentPosition;
        var name = ParseToken<UppercaseIdentifier>();
        var colon = name.Source.ParseToken<Colon>();
        var definition = colon.Source.ParseExpression();
        var end = definition.Source.CurrentPosition;

        return definition.WithValue(new FieldDeclaration(name.Result.Identifier, definition.Result, start, end));
    }
}