using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<Expression> ParseParentheses()
    {
        var start = CurrentPosition;
        var openParen = ParseToken<OpenParen>();
        var expression = openParen.Source.ParseExpression();
        var closeParen = expression.Source.ParseToken<CloseParen>();
        var end = closeParen.Source.CurrentPosition;

        return closeParen.WithValue(expression.Result with { Start = start, End = end });
    }
}