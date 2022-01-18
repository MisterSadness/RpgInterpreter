using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<ObjectCreation> ParseObjectCreation()
    {
        var start = CurrentPosition;
        var keyword = ParseToken<New>();
        var className = keyword.Source.ParseToken<UppercaseIdentifier>();
        var afterTraits = className.Source;
        TraitList? traits = null;
        if (className.Source.PeekOrDefault() is With)
        {
            var parsedTraits = className.Source.ParseTraits();
            afterTraits = parsedTraits.Source;
            traits = parsedTraits.Result;
        }

        var end = afterTraits.CurrentPosition;

        return new ParseResult<ObjectCreation>(afterTraits,
            new ObjectCreation(className.Result.Identifier, traits, start, end));
    }
}