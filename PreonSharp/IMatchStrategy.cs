using System.Collections.Frozen;

namespace PreonSharp;

public interface IMatchStrategy
{
    int Cost { get; }

    Task<QueryResult?> FindMatchAsync(string transformedName, FrozenDictionary<string, FrozenSet<NamespacedId>> normalizedNames);
}
