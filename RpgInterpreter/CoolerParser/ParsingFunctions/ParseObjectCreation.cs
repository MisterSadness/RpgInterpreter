using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<ObjectCreation> ParseObjectCreation()
    {
        var keyword = ParseToken<New>();

        var className = keyword.Source.ParseToken<UppercaseIdentifier>();

        var traits = className.Source.ParseTraits();

        return traits.WithValue(new ObjectCreation(className.Result.Identifier, traits.Result));
    }
}