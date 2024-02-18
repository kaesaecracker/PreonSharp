using System.Collections.Frozen;

namespace PreonSharp;

public interface IMatchStrategy
{
    int Cost { get; }

    QueryResult? FindMatch(string transformedName, FrozenDictionary<string, FrozenSet<NamespacedId>> normalizedNames, CancellationToken? token = null);
}
