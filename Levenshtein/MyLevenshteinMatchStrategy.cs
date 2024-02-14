using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PreonSharp;

namespace Levenshtein;

public sealed class MyLevenshteinMatchStrategy(IOptions<LevenshteinMatchOptions> options) : IMatchStrategy
{
    private readonly LevenshteinMatchOptions _options = options.Value;
    public int Cost => 1000;

    public async Task<QueryResult?> FindMatchAsync(string transformedName,
        FrozenDictionary<string, FrozenSet<NamespacedId>> normalizedNames)
    {
        var minDist = decimal.MaxValue;
        List<QueryResultEntry> minDistValues = [];

        var lockObj = new object();

        await Parallel.ForEachAsync(normalizedNames, _options.ParallelOptions, (pair, token) =>
        {
            var (otherName, otherIds) = pair;

            var distance = Math.Round(
                Levenshtein.CalculateDistance(transformedName, otherName)
                / (decimal)Math.Max(transformedName.Length, otherName.Length)
                , _options.Decimals);

            lock (lockObj)
            {
                if (distance < minDist)
                {
                    minDistValues.Clear();
                    minDist = distance;
                }

                if (distance == minDist)
                    minDistValues.Add(new QueryResultEntry(otherName, otherIds));
            }

            return ValueTask.CompletedTask;
        });


        if (minDist > _options.Threshold)
            return null;

        return new QueryResult(
            Type: MatchType.Partial,
            Entries: minDistValues,
            EditDistance: minDist
        );
    }
}