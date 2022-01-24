using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public virtual IParseResult<Root> ParseProgram()
    {
        var start = CurrentPosition;
        var elementsState = ParseTerminated<Statement, Semicolon, EndOfInput>(
            p => p.ParseStatement()
        );
        var elements = elementsState.Result;
        var end = elementsState.Source.CurrentPosition;

        return elementsState.WithValue(new Root(NodeList.From(elements), start, end));
    }
}