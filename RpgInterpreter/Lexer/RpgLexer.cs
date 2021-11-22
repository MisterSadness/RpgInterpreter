using RpgInterpreter.Lexer.InnerLexers;

namespace RpgInterpreter.Lexer
{
    public class RpgLexer : Lexer
    {
        private static readonly InnerLexer[] InnerLexers =
        {
            new DiceOrNaturalLexer(),
            new LowercaseWordLexer(),
            new UppercaseWordLexer(),
            new OperatorLexer(),
            new SeparatorLexer(),
            new StringLexer(),
            new WhitespaceLexer()
        };

        public RpgLexer() : base(InnerLexers) { }
    }
}