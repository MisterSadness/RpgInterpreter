using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.ParsingExceptions;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<TToken> ParseToken<TToken>() where TToken : Token
    {
        var topToken = PeekOrDefault();

        if (topToken is TToken token)
        {
            return new ParseResult<TToken>(Dequeue(), token);
        }

        throw new ExpectedTokenNotFoundException<TToken>(topToken, CurrentPosition);
    }
}