using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers
{
    public class WhitespaceLexer : InnerLexer
    {
        public override bool FirstCharacterMatches(char c) => char.IsWhiteSpace(c);
        public override Token Match(ICharSource source)
        {
            var c = source.Peek();
            while (c.HasValue && char.IsWhiteSpace(c.Value))
            {
                source.Pop();
                c = source.Peek();
            }

            return new Whitespace();
        }
    }
}