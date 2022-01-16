using RpgInterpreter.Lexer.Sources;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.NonTerminals;
using RpgInterpreter.Productions;
using RpgInterpreter.Tree;
using RpgInterpreter.Utils;

namespace RpgInterpreter.Parser;

public class Parser
{
    private readonly TokenSource _tokenSource;
    private readonly ParsingTable _parsingTable;
    private readonly Stack<Symbol> _stack;
    private readonly ParseTree _tree = new();

    public Parser(ICharSource source)
    {
        _tokenSource = TokenSource.FromCharSource(source);
        var productions = Reflection.GetInstancesOfAllTypesInheriting<Production>();

        _parsingTable = new ParsingTableGenerator(productions).CalculateParsingTable();

        _stack = new Stack<Symbol>();
        _tree.BeginSubtree();
        _stack.Push(new Syntax());
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
                (ProductionEnd p, _) => HandleProductionEnd(p),
                (Terminal<EndOfInput>, { Value: EndOfInput }) => Finish(),
                (NonTerminal nt, { }) => HandleNonTerminals(nt, input),
                (Terminal t, { }) => HandleTerminals(t, input),
                _ => new Failure()
            };
        }

        return state;
    }

    private ParsingResult HandleProductionEnd(ProductionEnd productionEnd)
    {
        _stack.Pop();
        _tree.EndSubtree(productionEnd.Start);
        return new Success();
    }

    private ParsingResult Finish()
    {
        _stack.Pop();
        _tree.EndSubtree(new Syntax());
        return new Success();
    }

    private ParsingResult HandleNonTerminals(NonTerminal left, PositionedToken right)
    {
        var rightType = right.Value.GetType();

        var option = _parsingTable.Find(left, rightType);
        _stack.Pop();
        _tree.BeginSubtree();
        _stack.Push(new ProductionEnd(left));

        if (!option.HasValue)
        {
            throw new UnexpectedTokenException(right);
        }

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

    private ParsingResult HandleTerminals(Terminal left, PositionedToken right)
    {
        if (left.TokenType != right.Value.GetType())
        {
            throw new ExpectedTokenNotFoundException(left, right);
        }

        _stack.Pop();
        _tree.AddLeaf(left, right);
        _tokenSource.Pop();
        return new Success();
    }
}

public class ParsingException : Exception
{
    public ParsingException(string message) : base(message)
    {
    }
}

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