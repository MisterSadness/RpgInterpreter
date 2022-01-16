using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser;
using RpgInterpreter.Utils;
using Assignment = RpgInterpreter.CoolerParser.Grammar.Assignment;
using If = RpgInterpreter.CoolerParser.Grammar.If;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<Block> ParseBlock()
    {
        var open = ParseToken<OpenBrace>();

        var inner = open.Source.ParseTerminated<IBlockInner, Semicolon, CloseBrace>(s => s.ParseBlockInner());
        var innerList = inner.Result.ToList();
        if (innerList.SkipLast(1).Any(x => x is not Assignment or If or FunctionInvocation))
        {
            throw new ParsingException("Invalid block with an expression outside of last position.");
        }

        var returnExpression = innerList.LastOrDefault(s => s is Expression) as Expression ?? new Unit();

        return inner.WithValue(new Block(NodeList.From(innerList), returnExpression));
    }

    public IParseResult<IBlockInner> ParseBlockInner()
    {
        if (Queue.PeekOrDefault() is Set)
        {
            return ParseAssignment();
        }

        return ParseExpression();
    }
}