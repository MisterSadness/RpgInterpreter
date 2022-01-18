using RpgInterpreter.Lexer.Tokens;
using If = RpgInterpreter.Parser.Grammar.If;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<If> ParseIf()
    {
        var start = CurrentPosition;
        var ifState = ParseToken<Lexer.Tokens.If>();
        var condition = ifState.Source.ParseExpression();
        var then = condition.Source.ParseToken<Then>();
        var trueValue = then.Source.ParseExpression();
        var elseState = trueValue.Source.ParseToken<Else>();
        var falseValue = elseState.Source.ParseExpression();
        var end = falseValue.Source.CurrentPosition;

        return falseValue.WithValue(new If(
            condition.Result,
            trueValue.Result,
            falseValue.Result,
            start, end
        ));
    }
}