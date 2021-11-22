using System;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers
{
    public class OperatorLexer : InnerLexer
    {
        public override bool FirstCharacterMatches(char c) => "-+*/=!<>|&.".Contains(c);

        public override Token Match(ICharSource source)
        {
            var previous = source.Pop() ?? throw new UnexpectedEndOfInputException();
            var next = source.Peek();

            return (previous, next) switch
            {
                ('=', '=') => PopAndReturn(new Equality()),
                ('!', '=') => PopAndReturn(new Inequality()),
                ('<', '=') => PopAndReturn(new LessOrEqual()),
                ('>', '=') => PopAndReturn(new GreaterOrEqual()),
                ('|', '|') => PopAndReturn(new BooleanOr()),
                ('&', '&') => PopAndReturn(new BooleanAnd()),
                ('+', '+') => PopAndReturn(new Concatenation()),
                ('+', _) => new Addition(),
                ('-', _) => new Minus(),
                ('*', _) => new Multiplication(),
                ('/', _) => new Division(),
                ('<', _) => new Less(),
                ('>', _) => new Greater(),
                ('.', _) => new Access(),
                ('=', _) => new Assignment(),
                (_, _) => throw new ArgumentOutOfRangeException() //FIXME
            };

            Token PopAndReturn(Token value)
            {
                source.Pop();
                return value;
            }
        }
    }
}