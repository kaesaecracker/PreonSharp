using System;
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
        var minDist = decimal.MaxValue;
        List<QueryResultEntry> minDistValues = [];

        var distObj = new LevenshteinSearch(transformedName);

        foreach (var pair in normalizedNames)
        {
            token?.ThrowIfCancellationRequested();
            var (otherName, otherIds) = pair;

            var distance = distObj.DistanceFrom(otherName)
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