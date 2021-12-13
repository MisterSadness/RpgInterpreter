using Optional;
using Optional.Collections;
using RpgInterpreter.Lexer;
using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.NonTerminals;
using RpgInterpreter.Productions;
using RpgInterpreter.Tokens;

namespace RpgInterpreter.Parser;

public class Parser
{
    private readonly TokenSource _tokenSource;
    private readonly ParsingTable _parsingTable;
    private readonly Stack<Symbol> _stack;

    public Parser(TokenSource tokenSource, ParsingTable parsingTable, Symbol startSymbol)
    {
        _tokenSource = tokenSource;
        _parsingTable = parsingTable;
        _stack = new Stack<Symbol>();
        _stack.Push(new Terminal<EndOfInput>());
        _stack.Push(startSymbol);
    }

    public virtual void Parse()
    {
        ParsingResult state = new Success();
        while (_stack.Any() && state is not Failure)
        {
            var top = _stack.Pop();
            var input = _tokenSource.Peek().Value;
            var inputType = input.GetType();
            state = (top, input) switch
            {
                (Terminal<EndOfInput>, EndOfInput) => new Success(),
                (NonTerminal nt, { }) => HandleNonTerminals(nt, inputType),
                (Terminal t, { }) => HandleTerminals(t, input),
                _ => new Failure()
            };
        }
    }

    private ParsingResult HandleNonTerminals(NonTerminal left, Type right)
    {
        var production = _parsingTable.Find(left, right);
        _stack.Pop();

        return new Success();
    }

    private ParsingResult HandleTerminals(Terminal left, Token right)
    {
        if (left.TokenType != right.GetType())
        {
            return new Failure();
        }

        _stack.Pop();
        _tokenSource.Pop();
        return new Success();
    }
}

public record ParsingResult;

public record Success : ParsingResult;

public record Failure : ParsingResult;

public class ParsingException : Exception { }

public class ParsingTable
{
    private readonly Dictionary<(NonTerminal, Type), Production> _table;
    public ParsingTable(Dictionary<(NonTerminal, Type), Production> table) => _table = table;

    public Option<Production> Find(NonTerminal nonTerminal, Type nextInputType) =>
        _table.GetValueOrNone((nonTerminal, nextInputType));
}

public record Symbol;

public abstract record Terminal : Symbol
{
    public abstract Type TokenType { get; }
}

public record Epsilon : Terminal<Whitespace>;

public record Terminal<T> : Terminal where T : Token
{
    public override Type TokenType { get; } = typeof(T);
}

public class TokenSource
{
    private readonly IEnumerator<PositionedToken> _tokens;

    public TokenSource(IEnumerable<PositionedToken> tokens) =>
        _tokens = tokens.GetEnumerator();

    public static TokenSource FromCharSource(ICharSource source) =>
        new(new TrackingRpgLexer().Tokenize(source));

    public PositionedToken Peek() => _tokens.Current;

    public PositionedToken Pop()
    {
        _tokens.MoveNext();
        return _tokens.Current;
    }
}

