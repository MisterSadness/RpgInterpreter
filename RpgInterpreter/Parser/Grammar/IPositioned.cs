namespace RpgInterpreter.Parser.Grammar;

public interface IPositioned
{
    Position Start { get; }
    Position End { get; }
}