using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreter.Parser.ParsingExceptions;
using Base = RpgInterpreter.Lexer.Tokens.Base;
using This = RpgInterpreter.Lexer.Tokens.This;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<Expression> ParseExpression(Precedence currentPrecedence = Precedence.None)
    {
        var parsedExpression = PeekOrDefault() switch
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
            UppercaseIdentifier => ParseObjectCreation(),
            OpenBrace => ParseBlock(),
            _ => throw new ParsingException($"Expected expression at {CurrentPosition.Formatted}.")
        };

        if (parsedExpression.Source.PeekOrDefault() is OpenParen)
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
        } while (parsedExpression.Source.PeekOrDefault() is Operator && parsedOperation != null);

        return parsedExpression;
    }
}