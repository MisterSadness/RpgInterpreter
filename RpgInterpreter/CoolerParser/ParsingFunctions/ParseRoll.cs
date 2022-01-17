using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<DiceRoll> ParseRoll(Expression expression)
    {
        var start = CurrentPosition;
        var openParen = ParseToken<OpenParen>();
        var closeParen = openParen.Source.ParseToken<CloseParen>();
        var end = closeParen.Source.CurrentPosition;

        return closeParen.WithValue(new DiceRoll(expression, start, end));
    }
}