using System;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers
{
    public class SeparatorLexer : InnerLexer
    {
        public override bool FirstCharacterMatches(char c) => "(){}[]:".Contains(c);
        public override Token Match(ICharSource source)
        {
            var c = source.Pop() ?? throw new UnexpectedEndOfInput();

            return c switch
            {
                '(' => new OpenParen(),
                ')' => new CloseParen(),
                '[' => new OpenBracket(),
                ']' => new CloseBracket(),
                '{' => new OpenBrace(),
                '}' => new CloseBrace(),
                ':' => new Colon(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}