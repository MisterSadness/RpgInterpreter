using RpgInterpreter.ExceptionHandler;
using RpgInterpreter.Parser.Grammar;

namespace RpgInterpreter.TypeChecker.SemanticExceptions;

public class IfConditionMustBeBooleanException : SemanticException, IIntervalPositionedException
{
    public IfConditionMustBeBooleanException(IfExpression ifExpression, Type conditionType) :
        base($"Conditional statement of an if expression must be a boolean, but was {conditionType}")
    {
        Start = ifExpression.Condition.Start;
        End = ifExpression.Condition.End;
    }

    public Position Start { get; }
    public Position End { get; }
}