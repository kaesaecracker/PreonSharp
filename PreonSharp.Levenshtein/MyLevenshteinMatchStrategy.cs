using System.Collections.Frozen;
using System.Collections.Generic;
using System.Threading;

namespace PreonSharp.Levenshtein;

public sealed class MyLevenshteinMatchStrategy : IMatchStrategy
{
    public int Cost => 1000;

    public QueryResult? FindMatch(string transformedName,
        FrozenDictionary<string, FrozenSet<string>> normalizedNames,
        CancellationToken? token = null)
    {
        var minDist = int.MaxValue;
        List<QueryResultEntry> minDistValues = [];

        var distObj = new LevenshteinSearch(transformedName);

        foreach (var pair in normalizedNames)
        {
            token?.ThrowIfCancellationRequested();
            var (otherName, otherIds) = pair;

            var distance = distObj.DistanceFrom(otherName);
            if (distance < minDist)
            {
                minDistValues.Clear();
                minDist = distance;
            }

            if (distance == minDist)
                minDistValues.Add(new QueryResultEntry(otherName, otherIds));
        }

        if (minDist == int.MaxValue)
            return null;

        var relativeDistance = minDist / (decimal)transformedName.Length;
        return new QueryResult(
            Type: MatchType.Partial,
            Results: minDistValues
        );
    }
}