using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<ObjectDeclaration> ParseObjectDeclaration()
    {
        var start = CurrentPosition;
        var name = ParseToken<UppercaseIdentifier>();

        var sourceState = name.Source;
        UppercaseIdentifier? baseName = null;
        TraitList? traitList = null;

        if (sourceState.PeekOrDefault() is Extends)
        {
            var extends = name.Source.ParseToken<Extends>();

            var baseState = extends.Source.ParseToken<UppercaseIdentifier>();

            sourceState = baseState.Source;
            baseName = baseState.Result;
        }

        if (sourceState.PeekOrDefault() is With)
        {
            var traits = sourceState.ParseTraits();

            sourceState = traits.Source;
            traitList = traits.Result;
        }

        var fieldsState = sourceState.ParseFields();
        var end = fieldsState.Source.CurrentPosition;

        return fieldsState.WithValue(new ObjectDeclaration(
            name.Result.Identifier,
            baseName?.Identifier,
            traitList,
            fieldsState.Result,
            start, end
        ));
    }
}