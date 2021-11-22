using RpgInterpreter.Lexer;

namespace RpgInterpreter.Tokens
{
    public abstract record Operator : Token;
    public record Addition : Operator;
    public record Minus : Operator;
    public record Multiplication : Operator;
    public record Division : Operator;
    public record Assignment : Operator;
    public record Equality : Operator;
    public record Inequality : Operator;
    public record Greater : Operator;
    public record Less : Operator;
    public record GreaterOrEqual : Operator;
    public record LessOrEqual : Operator;
    public record BooleanAnd : Operator;
    public record BooleanOr : Operator;
    public record Concatenation : Operator;
    public record Access : Operator;
}