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

            // Pop starting quote, the exception shouldn't happen if we chose this lexer
            var starting = source.Pop();
            if (starting is not '"')
            {
                throw new MissingOpeningQuote();
            }

            var c = source.Pop();
            while (c.HasValue)
            {
                if (c is '\\')
                {
                    c = MatchEscaped();
                    sb.Append(c);
                }
                else if (c is '"')
                {
                    break;
                }
                else
                {
                    sb.Append(c);
                }
                c = source.Pop();
            }

            if (c is not '"')
            {
                throw new MissingClosingQuote();
            }
            
            return new StringLiteral(sb.ToString());

            bool IsInnerString(char c)
            {
                return char.IsLetterOrDigit(c) || char.IsPunctuation(c) || c is ' ' or '\t';
            }

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