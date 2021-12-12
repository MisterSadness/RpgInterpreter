using Optional;

namespace RpgInterpreter.Utils
{
    public static class OptionalExtensions {
        public static Option<T> SomeWhen<T>(this T value, bool condition) => value.SomeWhen(x => condition);
    }
}
