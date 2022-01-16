using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<UnaryMinus> ParseUnaryMinus()
    {
        var minusState = ParseToken<Minus>();

        var expressionState = minusState.Source.ParseExpression(Precedence.UnaryMinus);
        var expression = expressionState.Result;

        return expressionState.WithValue(new UnaryMinus(expression));
    }
}