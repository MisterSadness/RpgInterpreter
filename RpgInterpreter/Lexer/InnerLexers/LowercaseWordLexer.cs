using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers;

public class LowercaseWordLexer : InnerLexer
{
    public override bool FirstCharacterMatches(char c) => char.IsLower(c);

    public override Token Match(ICharSource source)
    {
        var wholeString = MatchAll(source, IdentifierUtils.IsInnerIdentifier);

        return wholeString switch
        {
            "if" => new If(),
            "then" => new Then(),
            "else" => new Else(),
            "trait" => new Trait(),
            "for" => new For(),
            "with" => new With(),
            "extends" => new Extends(),
            "and" => new And(),
            "base" => new Base(),
            "this" => new This(),
            "fun" => new Fun(),
            "new" => new New(),
            "true" => new BooleanLiteral(true),
            "false" => new BooleanLiteral(false),
            _ => new LowercaseIdentifier(wholeString)
        };
    }
}