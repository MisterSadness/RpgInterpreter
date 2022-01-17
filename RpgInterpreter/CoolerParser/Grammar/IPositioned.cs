using RpgInterpreter.Lexer.Sources;

namespace RpgInterpreter.CoolerParser.Grammar;

public interface IPositioned
{
    Position Start { get; }
    Position End { get; }
}