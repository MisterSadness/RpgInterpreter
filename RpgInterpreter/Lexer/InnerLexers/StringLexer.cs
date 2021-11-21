using System.Text;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers
{
    public class StringLexer : InnerLexer
    {
        public override bool FirstCharacterMatches(char c) => c == '"';

        public override Token Match(ICharSource source)
        {
            var sb = new StringBuilder();

            // Pop starting quote
            var starting = source.Pop();
            if (starting is not '"')
            {
                throw new MissingQuote();
            }

            var c = source.Peek();
            while (c.HasValue)
            {
                source.Pop();
                if (c is '\\')
                {
                    c = MatchEscaped();
                }
                sb.Append(c);
                c = source.Peek();
            }

            // Pop ending quote
            var ending = source.Pop();
            if (ending is not '"')
            {
                throw new MissingQuote();
            }
            
            return new StringLiteral(sb.ToString());

            char MatchEscaped()
            {
                var next = source.Pop();
                return next switch
                {
                    'n' => '\n',
                    '"' => '"',
                    '\\' => '\\',
                    _ => throw new UndefinedEscapeSequence()
                };
            }
        }
    }
}