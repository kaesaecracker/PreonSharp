using System.Collections.Frozen;
using Fastenshtein;
using Microsoft.Extensions.Options;

namespace PreonSharp.Fastenshtein;

internal sealed class LevenshteinMatchStrategy(IOptions<LevenshteinMatchOptions> options) : IMatchStrategy
{
    private readonly LevenshteinMatchOptions _options = options.Value;

    public int Cost => 1000;

    public async Task<QueryResult?> FindMatchAsync(string transformedName,
        FrozenDictionary<string, FrozenSet<NamespacedId>> normalizedNames)
    {
        var minDist = decimal.MaxValue;
        List<QueryResultEntry> minDistValues = [];

        var distanceObj = new Levenshtein(transformedName);
        await foreach (var (otherName, otherIds) in normalizedNames.ToAsyncEnumerable())
        {
            var distance = distanceObj.DistanceFrom(otherName)
                           / (decimal)Math.Max(transformedName.Length, otherName.Length);
            distance = Math.Round(distance, _options.Decimals);

            if (distance < minDist)
            {
                minDistValues.Clear();
                minDist = distance;
            }

            if (distance == minDist)
                minDistValues.Add(new QueryResultEntry(otherName, otherIds));
        }

        if (minDist > _options.Threshold)
            return null;

        return new QueryResult(
            Type: MatchType.Partial,
            Entries: minDistValues,
            EditDistance: minDist
        );
    }
}
