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
                throw new MissingOpeningQuoteException();
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
                else if (IsInnerString(c.Value))
                {
                    sb.Append(c);
                }
                else
                {
                    throw new InvalidCharacterException();
                }
                c = source.Pop();
            }

            if (c is not '"')
            {
                throw new MissingClosingQuoteException();
            }
            
            return new StringLiteral(sb.ToString());

            bool IsInnerString(char x) => char.IsLetterOrDigit(x) || char.IsPunctuation(x) || x is ' ' or '\t';

            char MatchEscaped() => source.Pop() switch
            {
                'n' => '\n',
                '"' => '"',
                '\\' => '\\',
                _ => throw new UndefinedEscapeSequenceException()
            };
        }
    }
}