using RpgInterpreter.Lexer.Tokens;
using RpgInterpreter.Parser.Grammar;
using RpgInterpreter.Parser.ParsingExceptions;
using Assignment = RpgInterpreter.Parser.Grammar.Assignment;

namespace RpgInterpreter.Parser.ParsingFunctions;

public partial class SourceState
{
    public IParseResult<Assignment> ParseAssignment()
    {
        var start = CurrentPosition;
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

        var end = valueState.Source.CurrentPosition;
        var assignment = new Assignment(assignable, value, start, end);
        return valueState.WithValue(assignment);
    }
}