using Optional;
using Optional.Collections;
using RpgInterpreter.NonTerminals;
using RpgInterpreter.Productions;

namespace RpgInterpreter.Parser;

public class ParsingTable
{
    private readonly Dictionary<(NonTerminal, Type), Production> _table;
    public ParsingTable(Dictionary<(NonTerminal, Type), Production> table) => _table = table;

    public Option<Production> Find(NonTerminal nonTerminal, Type nextInputType) =>
        _table.GetValueOrNone((nonTerminal, nextInputType));
}