using System;
using System.Text;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers
{
    public abstract class InnerLexer
    {
        public abstract bool FirstCharacterMatches(char c);
        public abstract Token Match(ICharSource source);

        protected static string MatchAll(ICharSource source, Predicate<char> predicate)
        {
            var sb = new StringBuilder();
            var c = source.Peek();
            while (c.HasValue && predicate(c.Value))
            {
                source.Pop();
                sb.Append(c);
                c = source.Peek();
            }

            return sb.ToString();
        }
    }
}