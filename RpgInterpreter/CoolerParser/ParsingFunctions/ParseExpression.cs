using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser;
using RpgInterpreter.Utils;
using Base = RpgInterpreter.Lexer.Tokens.Base;
using If = RpgInterpreter.Lexer.Tokens.If;
using This = RpgInterpreter.Lexer.Tokens.This;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<Expression> ParseExpression(Precedence currentPrecedence = Precedence.None)
    {
        var parsedExpression = Queue.PeekOrDefault() switch
        {
            OpenBracket => ParseList(),
            NaturalLiteral => ParseNatural(),
            DiceLiteral => ParseDice(),
            BooleanLiteral => ParseBoolean(),
            StringLiteral => ParseString(),
            Minus => ParseUnaryMinus(),
            LowercaseIdentifier => ParseReference(),
            Base or This => ParseSelfReference(),
            OpenParen => ParseParentheses(),
            If => ParseIf(),
            New => ParseObjectCreation(),
            OpenBrace => ParseBlock(),
            _ => throw new ParsingException("Expected expression.")
        };

        if (parsedExpression.Source.Queue.PeekOrDefault() is OpenParen)
        {
            parsedExpression = parsedExpression.Source.ParseRoll(parsedExpression.Result);
        }

        var newPrecedence = currentPrecedence;
        IParseResult<BinaryOperation>? parsedOperation;
        do
        {
            parsedOperation =
                parsedExpression.Source.ParseBinaryOperation(parsedExpression.Result, newPrecedence);

            if (parsedOperation != null)
            {
                parsedExpression = parsedOperation;
            }
        } while (parsedExpression.Source.Queue.PeekOrDefault() is Operator && parsedOperation != null);

        return parsedExpression;
    }
}