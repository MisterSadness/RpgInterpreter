using System.Text;
using Optional.Unsafe;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers;

public class StringLexer : InnerLexer
{
    public override bool FirstCharacterMatches(char c) => c == '"';

    public override Token Match(ICharSource source)
    {
        var sb = new StringBuilder();

        // Pop starting quote, the exception shouldn't happen if we chose this lexer
        var starting = source.Pop().ToNullable();
        if (starting is null)
        {
            throw new UnexpectedEndOfInputException();
        }

        if (starting is not '"')
        {
            throw new UnexpectedInputException('"', starting.Value);
        }

        var c = source.Pop().ToNullable();
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
                throw new InvalidCharacterException(c.Value);
            }

            c = source.Pop().ToNullable();
        }

        if (c is not '"')
        {
            throw new MissingClosingQuoteException();
        }

        return new StringLiteral(sb.ToString());

        bool IsInnerString(char x) => char.IsLetterOrDigit(x) || char.IsPunctuation(x) || x is ' ' or '\t';

        char MatchEscaped()
        {
            return source.Pop().ToNullable() switch
            {
                'n' => '\n',
                't' => '\t',
                '"' => '"',
                '\\' => '\\',
                { } escaped => throw new UndefinedEscapeSequenceException(escaped),
                _ => throw new UnexpectedEndOfInputException()
            };
        }
    }
}