using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<List> ParseList()
    {
        var openBracketState = ParseToken<OpenBracket>();

        var elementsState = openBracketState.Source.ParseSeparated<Expression, Comma, CloseBracket>(
            p => p.ParseExpression()
        );
        var elements = elementsState.Result;

        return elementsState.WithValue(new List(NodeList.From(elements)));
    }

    public IParseResult<Natural> ParseNatural()
    {
        var parsed = ParseToken<NaturalLiteral>();

        return parsed.WithValue(new Natural(parsed.Result.Value));
    }

    public IParseResult<Dice> ParseDice()
    {
        var parsed = ParseToken<DiceLiteral>();

        return parsed.WithValue(new Dice(parsed.Result.Count, parsed.Result.Max));
    }

    public IParseResult<BooleanExpression> ParseBoolean()
    {
        var parsed = ParseToken<BooleanLiteral>();

        return parsed.WithValue(new BooleanExpression(parsed.Result.Value));
    }

    public IParseResult<StringExpression> ParseString()
    {
        var parsed = ParseToken<StringLiteral>();

        return parsed.WithValue(new StringExpression(parsed.Result.Value));
    }
}