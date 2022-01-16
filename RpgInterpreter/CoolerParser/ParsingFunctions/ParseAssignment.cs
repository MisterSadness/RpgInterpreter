using RpgInterpreter.CoolerParser.Grammar;
using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser;
using Assignment = RpgInterpreter.CoolerParser.Grammar.Assignment;

namespace RpgInterpreter.CoolerParser.ParsingFunctions;

public partial record SourceState
{
    public IParseResult<Assignment> ParseAssignment()
    {
        var keywordState = ParseToken<Set>();

        var nameState = keywordState.Source.ParseReference();
        var name = nameState.Result;

        if (name is not IAssignable assignable)
        {
            throw new ParsingException("Left side of an assignment must refer to a variable or a field.");
        }

        var equalSignState = nameState.Source.ParseToken<Lexer.Tokens.Assignment>();

        var valueState = equalSignState.Source.ParseExpression();
        var value = valueState.Result;

        var assignment = new Assignment(assignable, value);
        return valueState.WithValue(assignment);
    }
}