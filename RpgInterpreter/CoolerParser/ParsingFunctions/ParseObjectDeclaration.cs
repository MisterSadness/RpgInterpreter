using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Utils;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<ObjectDeclaration> ParseObjectDeclaration()
    {
        var name = ParseToken<UppercaseIdentifier>();

        var sourceState = name.Source;
        UppercaseIdentifier? baseName = null;
        TraitList? traitList = null;

        if (sourceState.Queue.PeekOrDefault() is Extends)
        {
            var extends = name.Source.ParseToken<Extends>();

            var baseState = extends.Source.ParseToken<UppercaseIdentifier>();

            sourceState = baseState.Source;
            baseName = baseState.Result;
        }

        if (sourceState.Queue.PeekOrDefault() is With)
        {
            var traits = sourceState.ParseTraits();

            sourceState = traits.Source;
            traitList = traits.Result;
        }

        var fieldsState = sourceState.ParseFields();

        return fieldsState.WithValue(new ObjectDeclaration(
            name.Result.Identifier,
            baseName?.Identifier,
            traitList,
            fieldsState.Result
        ));
    }
}