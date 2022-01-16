using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Utils;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<TraitDeclaration> ParseTraitDeclaration()
    {
        var keyword = ParseToken<Trait>();

        var name = keyword.Source.ParseToken<UppercaseIdentifier>();

        var state = name.Source;
        UppercaseIdentifier? baseName = null;

        if (state.Queue.PeekOrDefault() is For)
        {
            var forKeyword = name.Source.ParseToken<For>();

            var baseState = forKeyword.Source.ParseToken<UppercaseIdentifier>();

            state = baseState.Source;
            baseName = baseState.Result;
        }

        var fields = state.ParseFields();

        return fields.WithValue(new TraitDeclaration(name.Result.Identifier, baseName?.Identifier, fields.Result));
    }
}