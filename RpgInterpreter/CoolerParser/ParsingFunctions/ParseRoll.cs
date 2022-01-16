using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<DiceRoll> ParseRoll(Expression expression)
    {
        var openParen = ParseToken<OpenParen>();
        var closeParen = openParen.Source.ParseToken<CloseParen>();

        return closeParen.WithValue(new DiceRoll(expression));
    }
}