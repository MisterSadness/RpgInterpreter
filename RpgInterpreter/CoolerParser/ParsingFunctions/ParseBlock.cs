﻿using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.CoolerParser.ParsingExceptions;
using RpgInterpreter.Lexer.Tokens;
using Assignment = RpgInterpreter.CoolerParser.Grammar.Assignment;
using If = RpgInterpreter.CoolerParser.Grammar.If;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<Block> ParseBlock()
    {
        var start = CurrentPosition;
        var open = ParseToken<OpenBrace>();

        var inner = open.Source.ParseTerminated<IBlockInner, Semicolon, CloseBrace>(s => s.ParseBlockInner());
        var innerList = inner.Result.ToList();
        if (innerList.SkipLast(1).Any(x => x is not Assignment or If or FunctionInvocation))
        {
            throw new ParsingException("Invalid block with an expression outside of last position.");
        }

        var end = inner.Source.CurrentPosition;

        var returnExpressionPosition =
            (innerList.LastOrDefault() as IPositioned)?.Start ?? end;

        var returnExpression = innerList.LastOrDefault(s => s is Expression) as Expression ??
                               new Unit(returnExpressionPosition, end);

        return inner.WithValue(new Block(NodeList.From(innerList), returnExpression, start, end));
    }

    public IParseResult<IBlockInner> ParseBlockInner()
    {
        if (PeekOrDefault() is Set)
        {
            return ParseAssignment();
        }

        return ParseExpression();
    }
}