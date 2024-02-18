using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace PreonSharp.Levenshtein;

public sealed class MyLevenshteinMatchStrategy : IMatchStrategy
{
    public int Cost => 1000;

    public QueryResult? FindMatch(string transformedName,
        FrozenDictionary<string, FrozenSet<NamespacedId>> normalizedNames)
    {
        var minDist = decimal.MaxValue;
        List<QueryResultEntry> minDistValues = [];

        var distObj = new LevenshteinSearch(transformedName);

        foreach (var pair in normalizedNames)
        {
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
            Entries: minDistValues,
            EditDistance: minDist
        );
    }
}