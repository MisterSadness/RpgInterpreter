using RpgInterpreter.Lexer;

namespace RpgInterpreter.Tokens
{
    abstract record Operator : Token;
    record Addition : Operator;
    record Minus : Operator;
    record Multiplication : Operator;
    record Division : Operator;
    record Assignment : Operator;
    record Equality : Operator;
    record Inequality : Operator;
    record Greater : Operator;
    record Less : Operator;
    record GreaterOrEqual : Operator;
    record LessOrEqual : Operator;
    record BooleanAnd : Operator;
    record BooleanOr : Operator;
    record Concatenation : Operator;
    record Access : Operator;
}