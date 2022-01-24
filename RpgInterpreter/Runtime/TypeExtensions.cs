using RpgInterpreter.Runtime.RuntimeExceptions;
using RpgInterpreter.TypeChecker;
using Type = RpgInterpreter.TypeChecker.Type;

namespace RpgInterpreter.Runtime;

public static class TypeExtensions
{
    public static FunctionType AsFunctionType(this Type type) =>
        type as FunctionType ?? throw new InterpreterImplementationException();

    public static ObjectType AsObjectType(this Type type) =>
        type as ObjectType ?? throw new InterpreterImplementationException();
}