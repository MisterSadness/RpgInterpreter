using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public virtual IParseResult<Root> ParseProgram()
    {
        var elementsState = ParseTerminated<Statement, Semicolon, EndOfInput>(
            p => p.ParseStatement()
        );
        var elements = elementsState.Result;

        return elementsState.WithValue(new Root(NodeList.From(elements)));
    }
}