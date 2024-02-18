using System.Collections.Frozen;
using System.Threading;
using Fastenshtein;

namespace PreonSharp.Fastenshtein;

internal sealed class LevenshteinMatchStrategy : IMatchStrategy
{
    public int Cost => 1000;

    public QueryResult? FindMatch(string transformedName,
        FrozenDictionary<string, FrozenSet<string>> normalizedNames,
        CancellationToken? token = null)
    {
        var minDist = decimal.MaxValue;
        List<QueryResultEntry> minDistValues = [];

        var distanceObj = new Levenshtein(transformedName);
        foreach (var (otherName, otherIds) in normalizedNames)
        {
            token?.ThrowIfCancellationRequested();
            
            var distance = distanceObj.DistanceFrom(otherName)
                           / (decimal)Math.Max(transformedName.Length, otherName.Length);

            if (distance < minDist)
            {
                minDistValues.Clear();
                minDist = distance;
            }

            if (distance == minDist)
                minDistValues.Add(new QueryResultEntry(otherName, otherIds));
        }

        if (minDist == decimal.MaxValue)
            return null;

        return new QueryResult(
            Type: MatchType.Partial,
            FoundIds: minDistValues,
            EditDistance: minDist
        );
    }
}