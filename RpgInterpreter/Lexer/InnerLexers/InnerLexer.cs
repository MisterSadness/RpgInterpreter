using System.Text;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers;

public abstract class InnerLexer
{
    public abstract bool FirstCharacterMatches(char c);
    public abstract Token Match(ICharSource source);

    protected static string MatchAll(ICharSource source, Func<char, bool> predicate)
    {
        var sb = new StringBuilder();
        var option = source.Peek();
        while (option.Exists(predicate))
        {
            source.Pop().MatchSome(c => sb.Append(c));
            option = source.Peek();
        }

        return sb.ToString();
    }
}