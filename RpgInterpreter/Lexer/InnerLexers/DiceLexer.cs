using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer.InnerLexers
{
    public class DiceLexer : InnerLexer
    {
        public override bool FirstCharacterMatches(char c) => char.IsDigit(c) || c =='d';
        public override Token Match(ICharSource source)
        {
            var first = source.Peek();
            if (first is 'd')
            {
                return new Dice(1, MatchInt());
            }
            
            var firstInt = MatchInt();
            var separator = source.Peek();

            if (separator == null)
            {
                return new Dice(firstInt, 1);
            }

            var secondInt = MatchInt();
            return new Dice(firstInt, secondInt);

            int MatchInt()
            {
                var str = MatchAll(source, char.IsDigit);
                return int.Parse(str);
            }
        }
    }
}