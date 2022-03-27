using System.Collections.Immutable;
using RpgInterpreter.TypeChecker;

namespace RpgInterpreter.Runtime;

public class Function
{
    public Function(Call body, FunctionType type)
    {
        Body = body;
        Type = type;
    }

    public delegate Value Call(InterpreterState state);

    public Call Body { get; }
    public FunctionType Type { get; init; }
    public IImmutableList<FunctionParameter> Parameters => Type.ParameterTypes;
    public string Name => Type.Name;
}