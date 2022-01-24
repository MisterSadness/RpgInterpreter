using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<UnaryMinus> ParseUnaryMinus()
    {
        var start = CurrentPosition;
        var minusState = ParseToken<Minus>();
        var expressionState = minusState.Source.ParseExpression(Precedence.UnaryMinus);
        var end = expressionState.Source.CurrentPosition;

        return expressionState.WithValue(new UnaryMinus(expressionState.Result, start, end));
    }
}