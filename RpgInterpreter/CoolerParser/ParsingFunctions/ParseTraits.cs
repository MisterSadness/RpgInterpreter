using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<TraitList> ParseTraits()
    {
        var start = CurrentPosition;
        var with = ParseToken<With>();

        var firstTrait = with.Source.ParseToken<UppercaseIdentifier>();

        var traitList = new List<string> { firstTrait.Result.Identifier };
        var state = firstTrait.Source;
        while (state.PeekOrDefault() is And)
        {
            var and = state.ParseToken<And>();

            var trait = and.Source.ParseToken<UppercaseIdentifier>();
            traitList.Add(trait.Result.Identifier);
            state = trait.Source;
        }

        var end = state.CurrentPosition;

        return new ParseResult<TraitList>(state, new TraitList(NodeList.From(traitList), start, end));
    }
}