using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.NonTerminals;
using RpgInterpreter.Productions;
using RpgInterpreter.Tokens;
using RpgInterpreter.Utils;

namespace RpgInterpreter.Parser;

public class Parser
{
    private readonly TokenSource _tokenSource;
    private readonly ParsingTable _parsingTable;
    private readonly Stack<Symbol> _stack;

    public Parser(ICharSource source)
    {
        _tokenSource = TokenSource.FromCharSource(source);
        var productions = Reflection.GetInstancesOfAllTypesInheriting<Production>();

        _parsingTable = new ParsingTableGenerator(productions).CalculateParsingTable();

        _stack = new Stack<Symbol>();
        _stack.Push(new Terminal<EndOfInput>());
        _stack.Push(new Syntax());
    }

    public Parser(TokenSource tokenSource, ParsingTable parsingTable, Symbol startSymbol)
    {
        _tokenSource = tokenSource;
        _parsingTable = parsingTable;
        _stack = new Stack<Symbol>();
        _stack.Push(new Terminal<EndOfInput>());
        _stack.Push(startSymbol);
    }

    public ParsingResult Parse()
    {
        ParsingResult state = new Success();
        while (_stack.Any() && state is not Failure)
        {
            var top = _stack.Peek();
            var input = _tokenSource.Peek();
            state = (top, input) switch
            {
                (Terminal<EndOfInput>, { Value: EndOfInput }) => Finish(),
                (NonTerminal nt, { }) => HandleNonTerminals(nt, input),
                (Terminal t, { }) => HandleTerminals(t, input),
                _ => new Failure()
            };
        }

        return state;
    }

    private ParsingResult Finish()
    {
        _stack.Pop();
        return new Success();
    }

    private ParsingResult HandleNonTerminals(NonTerminal left, PositionedToken right)
    {
        var rightType = right.Value.GetType();

        var option = _parsingTable.Find(left, rightType);
        _stack.Pop();

        if (option.HasValue)
        {
            option.MatchSome(production =>
            {
                if (production.RightSide.Length != 1 || production.RightSide.Single() is not Epsilon)
                {
                    foreach (var symbol in production.RightSide.Reverse())
                        _stack.Push(symbol);
                }

                Console.WriteLine(production.Formatted);
            });
            return new Success();
        }

        throw new UnexpectedTokenException(right);
    }

    private ParsingResult HandleTerminals(Terminal left, PositionedToken right)
    {
        if (left.TokenType != right.Value.GetType())
        {
            throw new ExpectedTokenNotFoundException(left, right);
        }

        _stack.Pop();
        _tokenSource.Pop();
        return new Success();
    }
}

public class ParsingException : Exception { }

public class UnexpectedTokenException : Exception
{
    public UnexpectedTokenException(PositionedToken unexpected) : base(
        $"An unexpected token {unexpected.Value} found at position {unexpected.Start.Formatted}.") { }
}

public class ExpectedTokenNotFoundException : Exception
{
    public ExpectedTokenNotFoundException(Terminal expected, PositionedToken actual) : base(
        $"Expected to find {expected} but found {actual.Value} at position {actual.Start.Formatted}.") { }
}