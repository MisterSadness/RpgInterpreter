using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<Expression> ParseParentheses()
    {
        var openParen = ParseToken<OpenParen>();

        var expression = openParen.Source.ParseExpression();

        var closeParen = expression.Source.ParseToken<CloseParen>();

        return closeParen.WithValue(expression.Result);
    }
}