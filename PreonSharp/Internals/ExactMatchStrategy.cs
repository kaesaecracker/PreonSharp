using System.Collections.Frozen;

namespace PreonSharp.Internals;

internal sealed class ExactMatchStrategy : IMatchStrategy
{
    public int Cost => 0;

    public QueryResult? FindMatch(string transformedName,
        FrozenDictionary<string, FrozenSet<NamespacedId>> normalizedNames)
    {
        if (!normalizedNames.TryGetValue(transformedName, out var foundIds))
            return null;

        return new QueryResult(Type: MatchType.Exact, Entries:
        [
            new QueryResultEntry(transformedName, foundIds)
        ]);
    }
}
