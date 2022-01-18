using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Parser.ParsingFunctions;

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