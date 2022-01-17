using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<ObjectCreation> ParseObjectCreation()
    {
        var start = CurrentPosition;
        var keyword = ParseToken<New>();
        var className = keyword.Source.ParseToken<UppercaseIdentifier>();
        var traits = className.Source.ParseTraits();
        var end = traits.Source.CurrentPosition;

        return traits.WithValue(new ObjectCreation(className.Result.Identifier, traits.Result, start, end));
    }
}