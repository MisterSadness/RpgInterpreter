using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers;

public class DiceOrNaturalLexer : InnerLexer
{
    public override bool FirstCharacterMatches(char c) => char.IsDigit(c);

    public override Token Match(ICharSource source)
    {
        var firstInt = MatchInt();
        var separator = source.Peek();

        if (!separator.Exists(c => c == 'd'))
        {
            return new NaturalLiteral(firstInt);
        }

        source.Pop();
        var secondInt = MatchInt();
        return new DiceLiteral(firstInt, secondInt);

        int MatchInt()
        {
            var str = MatchAll(source, char.IsDigit);

            if (int.TryParse(str, out var result))
            {
                return result;
            }

            throw new ExpectedIntegerLiteralException();
        }
    }
}