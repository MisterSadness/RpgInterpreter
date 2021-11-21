namespace RpgInterpreter.Lexer.Sources
{
    // Has to be struct because nullable reference types and value types are not compatible with each other
    public interface ISource<T> where T : struct
    {
        T? Peek();
        T? Pop();
    }
}