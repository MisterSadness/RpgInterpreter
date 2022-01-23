using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<IfExpression> ParseIf()
    {
        var start = CurrentPosition;
        var ifState = ParseToken<If>();
        var condition = ifState.Source.ParseExpression();
        var then = condition.Source.ParseToken<Then>();
        var trueValue = then.Source.ParseExpression();
        var elseState = trueValue.Source.ParseToken<Else>();
        var falseValue = elseState.Source.ParseExpression();
        var end = falseValue.Source.CurrentPosition;

        return falseValue.WithValue(new IfExpression(
            condition.Result,
            trueValue.Result,
            falseValue.Result,
            start, end
        ));
    }
}