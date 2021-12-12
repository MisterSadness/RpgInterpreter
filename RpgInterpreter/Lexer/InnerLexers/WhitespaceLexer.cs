using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers;

public class WhitespaceLexer : InnerLexer
{
    public override bool FirstCharacterMatches(char c) => char.IsWhiteSpace(c);

    public override Token Match(ICharSource source)
    {
        var option = source.Peek();

        if (!option.Exists(char.IsWhiteSpace))
        {
            throw new UnexpectedInputException();
        }

        while (option.Exists(char.IsWhiteSpace))
        {
            source.Pop();
            option = source.Peek();
        }

        return new Whitespace();
    }
}