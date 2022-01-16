using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Utils;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<TraitList> ParseTraits()
    {
        var with = ParseToken<With>();

        var firstTrait = with.Source.ParseToken<UppercaseIdentifier>();

        var traitList = new List<string> { firstTrait.Result.Identifier };
        var state = firstTrait.Source;
        while (state.Queue.PeekOrDefault() is And)
        {
            var and = state.ParseToken<And>();

            var trait = and.Source.ParseToken<UppercaseIdentifier>();
            traitList.Add(trait.Result.Identifier);
            state = trait.Source;
        }

        return new ParseResult<TraitList>(state, new TraitList(NodeList.From(traitList)));
    }
}