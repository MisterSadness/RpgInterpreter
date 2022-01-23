using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<ListExp> ParseList()
    {
        var start = CurrentPosition;
        var openBracketState = ParseToken<OpenBracket>();
        var elementsState = openBracketState.Source.ParseSeparated<Expression, Comma, CloseBracket>(
            p => p.ParseExpression()
        );
        var elements = elementsState.Result;
        var end = elementsState.Source.CurrentPosition;

        return elementsState.WithValue(new ListExp(NodeList.From(elements), start, end));
    }

    public IParseResult<Natural> ParseNatural()
    {
        var start = CurrentPosition;
        var parsed = ParseToken<NaturalLiteral>();
        var end = parsed.Source.CurrentPosition;

        return parsed.WithValue(new Natural(parsed.Result.Value, start, end));
    }

    public IParseResult<DiceExpression> ParseDice()
    {
        var start = CurrentPosition;
        var parsed = ParseToken<DiceLiteral>();
        var end = parsed.Source.CurrentPosition;

        return parsed.WithValue(new DiceExpression(parsed.Result.Count, parsed.Result.Max, start, end));
    }

    public IParseResult<BooleanExpression> ParseBoolean()
    {
        var start = CurrentPosition;
        var parsed = ParseToken<BooleanLiteral>();
        var end = parsed.Source.CurrentPosition;

        return parsed.WithValue(new BooleanExpression(parsed.Result.Value, start, end));
    }

    public IParseResult<StringExpression> ParseString()
    {
        var start = CurrentPosition;
        var parsed = ParseToken<StringLiteral>();
        var end = parsed.Source.CurrentPosition;

        return parsed.WithValue(new StringExpression(parsed.Result.Value, start, end));
    }
}