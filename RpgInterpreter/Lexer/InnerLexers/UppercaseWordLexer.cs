using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers;

public class UppercaseWordLexer : InnerLexer
{
    public override bool FirstCharacterMatches(char c) => char.IsUpper(c);

    public override Token Match(ICharSource source)
    {
        var wholeString = MatchAll(source, IdentifierUtils.IsInnerIdentifier);
        return new UppercaseIdentifier(wholeString);
    }
}