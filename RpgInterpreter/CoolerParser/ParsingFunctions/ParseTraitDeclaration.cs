using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<TraitDeclaration> ParseTraitDeclaration()
    {
        var start = CurrentPosition;
        var keyword = ParseToken<Trait>();
        var name = keyword.Source.ParseToken<UppercaseIdentifier>();

        var state = name.Source;
        UppercaseIdentifier? baseName = null;
        if (state.PeekOrDefault() is For)
        {
            var forKeyword = name.Source.ParseToken<For>();
            var baseState = forKeyword.Source.ParseToken<UppercaseIdentifier>();

            state = baseState.Source;
            baseName = baseState.Result;
        }

        var fields = state.ParseFields();
        var end = fields.Source.CurrentPosition;

        return fields.WithValue(new TraitDeclaration(name.Result.Identifier, baseName?.Identifier, fields.Result, start,
            end));
    }
}