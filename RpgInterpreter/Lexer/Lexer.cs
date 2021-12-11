using RpgInterpreter.Lexer.InnerLexers;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer;

public class Lexer
{
    private readonly IEnumerable<InnerLexer> _inner;

    public Lexer(IEnumerable<InnerLexer> inner) => _inner = inner;

    public virtual IEnumerable<Token> Tokenize(ICharSource source)
    {
        var c = source.Peek();
        while (c.HasValue)
        {
            var possible = _inner.SingleOrDefault(i => i.FirstCharacterMatches(c.Value))
                           ?? throw new UnexpectedInputException();
            yield return possible.Match(source);
            c = source.Peek();
        }
    }
}