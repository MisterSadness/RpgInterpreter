using System.Collections;
using System.Collections.Immutable;

namespace RpgInterpreter.TypeChecker;

public record Scope(IImmutableDictionary<string, Type> Types) : IEnumerable<KeyValuePair<string, Type>>
{
    public IEnumerator<KeyValuePair<string, Type>> GetEnumerator() => Types.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Types).GetEnumerator();

    public int Count => Types.Count;

    public bool ContainsKey(string key) => Types.ContainsKey(key);

    public bool TryGetValue(string key, out Type? value) => Types.TryGetValue(key, out value);

    public Type this[string key] => Types[key];

    public IEnumerable<string> Keys => Types.Keys;

    public IEnumerable<Type> Values => Types.Values;

    public Scope Add(string key, Type value) => new(Types.Add(key, value));

    public Scope AddRange(IEnumerable<KeyValuePair<string, Type>> pairs) => new(Types.AddRange(pairs));

    public bool TryGetKey(string equalKey, out string actualKey) => Types.TryGetKey(equalKey, out actualKey);

    public Type GetTypeOf(string key) =>
        GetValueOrDefault(key) ?? throw new InvalidOperationException($"No type for {key} in current scope.");

    public Type? GetValueOrDefault(string key) =>
        TryGetValue(key, out var value) ? value : null;

    public Scope SetItem(string key, Type value) => this with { Types = Types.SetItem(key, value) };
}