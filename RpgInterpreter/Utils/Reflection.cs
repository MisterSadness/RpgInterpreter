using System.Reflection;

namespace RpgInterpreter.Utils;

public static class Reflection
{
    public static IEnumerable<T> GetInstancesOfAllTypesInheriting<T>()
    {
        var types = GetAllConstructibleTypesInheriting<T>();
        var ctors = types.Select(GetConstructor);

        return ctors.Select(c => (T)c.Invoke(null));
    }

    public static IEnumerable<Type> GetAllConstructibleTypesInheriting<T>() =>
        typeof(T).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(T)) && !t.IsAbstract && !t.ContainsGenericParameters);

    public static IEnumerable<object> CreateInstanceOfGenericTypeForAllTypeParametersInheriting<TParameterBase>(
        Type genericType)
    {
        var typeParameters = GetAllConstructibleTypesInheriting<TParameterBase>();
        var genericTypeDefinition = genericType.GetGenericTypeDefinition();
        var constructedTypes = typeParameters.Select(tp => genericTypeDefinition.MakeGenericType(tp));
        var constructors = constructedTypes.Select(t => GetConstructor(t));

        return constructors.Select(c => c.Invoke(null));
    }

    public static ConstructorInfo GetConstructor(Type t) =>
        t.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Array.Empty<Type>());
}