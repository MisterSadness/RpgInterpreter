using System.Collections.Generic;
using RpgInterpreter.Lexer.LexingErrors;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Lexer
{
    // For some reason it's impossible to use yield return inside a try-catch statement
    // https://github.com/dotnet/csharplang/discussions/765
    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/yield#exception-handling
    public class TrackingRpgLexer : RpgLexer
    {
        public override IEnumerable<Token> Tokenize(ICharSource source)
        {
            var trackingSource = new TrackingCharSource(source);
            using var enumerator = base.Tokenize(trackingSource).GetEnumerator();

            while (true)
            {
                try
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                }
                catch (LexingException exception)
                {
                    var position = trackingSource.Position;
                    var annotated = new PositionedLexingException(exception, position);
                    throw annotated;
                }

                yield return enumerator.Current;
            }
        }
    }
}