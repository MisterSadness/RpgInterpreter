using Optional;

namespace RpgInterpreter;

// Has to be struct because nullable reference types and value types are not compatible with each other
public interface ISource<T>
{
    Option<T> Peek();
    Option<T> Pop();
}