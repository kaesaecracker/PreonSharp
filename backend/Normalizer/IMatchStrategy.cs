using System.Collections.Frozen;

namespace Normalizer;

public interface IMatchStrategy
{
    int Cost { get; }

    QueryResult? FindMatch(string transformedName, FrozenDictionary<string, FrozenSet<string>> normalizedNames, CancellationToken? token = null);
}
