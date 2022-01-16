using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser;
using RpgInterpreter.Utils;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<TToken> ParseToken<TToken>() where TToken : Token
    {
        var topToken = Queue.FirstOrDefault();

        if (topToken is TToken token)
        {
            return new ParseResult<TToken>(Queue.Dequeue().ToState(), token);
        }

        throw new ParsingException($"Expected token {typeof(TToken).Name}");
    }
}