using System.Collections.Generic;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer
{
    public class TrackingRpgLexer : RpgLexer
    {
        public override IEnumerable<Token> Tokenize(ICharSource source)
        {
            var trackingSource = new TrackingCharSource(source);
            try
            {
                return base.Tokenize(trackingSource);
            }
            catch (LexingException exception)
            {
                var position = trackingSource.Position;
                var annotated = new PositionedLexingException(exception, position);
                throw annotated;
            }
        }
    }
}