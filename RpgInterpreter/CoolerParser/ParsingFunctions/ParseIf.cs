using RpgInterpreter.Lexer.Tokens;
using If = RpgInterpreter.CoolerParser.Grammar.If;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<If> ParseIf()
    {
        var ifState = ParseToken<Lexer.Tokens.If>();

        var condition = ifState.Source.ParseExpression();

        var then = condition.Source.ParseToken<Then>();

        var trueValue = then.Source.ParseExpression();

        var elseState = trueValue.Source.ParseToken<Else>();

        var falseValue = elseState.Source.ParseExpression();

        return falseValue.WithValue(new If(
            condition.Result,
            trueValue.Result,
            falseValue.Result
        ));
    }
}