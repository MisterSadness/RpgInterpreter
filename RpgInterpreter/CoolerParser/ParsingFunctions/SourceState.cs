using System.Collections.Immutable;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Utils;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial class SourceState
{
    public SourceState(IImmutableQueue<PositionedToken> queue) => Queue = queue;
    public Position CurrentPosition => Queue.Peek().Start;
    public Token? PeekOrDefault() => Queue.PeekOrDefault()?.Value;
    public PositionedToken? PeekPositionedOrDefault => Queue.PeekOrDefault();
    private IImmutableQueue<PositionedToken> Queue { get; }
    public SourceState Dequeue() => new(Queue.Dequeue());
}