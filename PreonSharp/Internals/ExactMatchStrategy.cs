using System.Collections.Frozen;

namespace PreonSharp.Internals;

internal sealed class ExactMatchStrategy : IMatchStrategy
{
    public int Cost => 0;

    public QueryResult? FindMatch(
        string transformedName,
        FrozenDictionary<string, FrozenSet<string>> normalizedNames,
        CancellationToken? token = null)
    {
        if (!normalizedNames.TryGetValue(transformedName, out var foundIds))
            return null;

        return new QueryResult(Type: MatchType.Exact, FoundIds:
        [
            new QueryResultEntry(transformedName, foundIds)
        ]);
    }
}