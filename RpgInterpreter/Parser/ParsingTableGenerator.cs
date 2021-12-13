using Optional.Collections;
using RpgInterpreter.NonTerminals;
using RpgInterpreter.Productions;
using RpgInterpreter.Tokens;
using RpgInterpreter.Utils;

namespace RpgInterpreter.Parser;

public record ParsingTableGenerator(IEnumerable<Production> Productions)
{
    private Dictionary<NonTerminal, HashSet<Terminal>> _firsts = new();
    private Dictionary<NonTerminal, HashSet<Terminal>> _follows = new();

    public Dictionary<NonTerminal, HashSet<Terminal>> CalculateFirst()
    {
        var change = true;
        var result = new Dictionary<NonTerminal, HashSet<Terminal>>();
        while (change)
        {
            change = false;
            foreach (var production in Productions)
            {
                var left = production.LeftSide;
                var first = result.GetValueOrDefault(left, new HashSet<Terminal>());

                foreach (var symbol in production.RightSide)
                {
                    if (symbol is Terminal terminal)
                    {
                        change |= first.Add(terminal);
                        break;
                    }

                    if (symbol is NonTerminal nonTerminal)
                    {
                        var rightFirst = result.GetValueOrDefault(nonTerminal, new HashSet<Terminal>());
                        if (rightFirst.All(s => s is not Epsilon))
                        {
                            var preCount = first.Count;
                            first.UnionWith(rightFirst);
                            var postCount = first.Count;
                            change |= preCount != postCount;
                            break;
                        }
                    }
                }

                result[left] = first;
            }
        }

        _firsts = result;
        return result;
    }

    public Dictionary<NonTerminal, HashSet<Terminal>> CalculateFollow()
    {
        var change = true;
        var result = new Dictionary<NonTerminal, HashSet<Terminal>>
        {
            [new Syntax()] = new() { new Terminal<EndOfInput>() }
        };
        while (change)
        {
            change = false;
            foreach (var production in Productions)
            {
                var left = production.LeftSide;
                var leftFollow = result.GetValueOrDefault(left, new HashSet<Terminal>());

                foreach (var (symbol, position) in production.RightSide.Select((symbol, i) => (symbol, i)))
                {
                    if (symbol is NonTerminal nonTerminal)
                    {
                        var postfix = production.RightSide.Skip(position + 1).ToList();
                        var rightFollow = result.GetValueOrDefault(nonTerminal, new HashSet<Terminal>());

                        if (!postfix.Any())
                        {
                            var preCount = rightFollow.Count;
                            rightFollow.UnionWith(leftFollow);
                            if (nonTerminal == new Next6())
                            {
                                ;
                            }

                            var postCount = rightFollow.Count;
                            change |= preCount != postCount;
                            result[nonTerminal] = rightFollow;
                            continue;
                        }

                        var postfixHead = postfix.First();
                        var postfixFirst = new HashSet<Terminal>();
                        if (postfixHead is Terminal term and not Epsilon)
                        {
                            postfixFirst.Add(term);
                        }
                        else if (postfixHead is NonTerminal nt)
                        {
                            postfixFirst = _firsts.GetValueOrDefault(nt, new HashSet<Terminal>());
                        }

                        foreach (var t in postfixFirst.Where(t => t is not Epsilon))
                            change |= rightFollow.Add(t);

                        if (postfixFirst.Any(t => t is Epsilon))
                        {
                            var preCount = rightFollow.Count;
                            rightFollow.UnionWith(leftFollow);
                            var postCount = rightFollow.Count;
                            change |= preCount != postCount;
                        }

                        result[nonTerminal] = rightFollow;
                    }
                }
            }
        }

        _follows = result;
        return result;
    }

    private IEnumerable<Terminal> GetFirst(IEnumerable<Symbol> rightSide)
    {
        var head = rightSide.First();
        if (head is Terminal term)
        {
            return new List<Terminal> { term };
        }

        if (head is NonTerminal nt)
        {
            return _firsts.GetValueOrDefault(nt, new HashSet<Terminal>());
        }

        throw new YouFuckedUpYourGrammarException();
    }

    public ParsingTable CalculateParsingTable()
    {
        CalculateFirst();
        CalculateFollow();

        var nonTerminals = Reflection.GetInstancesOfAllTypesInheriting<NonTerminal>().ToList();
        var terminals = Reflection.CreateInstanceOfGenericTypeForAllTypeParametersInheriting<Token>(typeof(Terminal<>));

        var resultTable = new Dictionary<(NonTerminal, Type), Production>();

        foreach (var tokenType in terminals.Cast<Terminal>().Select(t => t.TokenType))
        foreach (var nonTerminal in nonTerminals)
        foreach (var production in Productions.Where(p => p.LeftSide == nonTerminal))
        {
            var first = GetFirst(production.RightSide).ToList();
            if (first.Any(t => t.TokenType == tokenType))
            {
                if (resultTable.ContainsKey((nonTerminal, tokenType)))
                {
                    throw new YouFuckedUpYourGrammarException();
                }

                resultTable[(nonTerminal, tokenType)] = production;
            }
            else if (first.Any(s => s is Epsilon)
                     && _follows.GetValueOrNone(nonTerminal).Exists(set => set.Any(t => t.TokenType == tokenType)))
            {
                if (resultTable.ContainsKey((nonTerminal, tokenType)))
                {
                    throw new YouFuckedUpYourGrammarException();
                }

                resultTable[(nonTerminal, tokenType)] = production;
            }
        }

        return new ParsingTable(resultTable);
    }
}

public class YouFuckedUpYourGrammarException : Exception { }