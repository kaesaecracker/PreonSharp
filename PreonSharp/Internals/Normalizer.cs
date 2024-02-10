using System.Collections.Frozen;
using Fastenshtein;

namespace PreonSharp.Internals;

internal sealed class Normalizer : INormalizer
{
    private readonly FrozenDictionary<string, FrozenSet<NamespacedId>> _normalizedNames;
    private readonly ILogger<Normalizer> _logger;
    private readonly INameTransformer _nameTransformer;

    public Normalizer(KnowledgeAggregator knowledgeAggregator,
        ILogger<Normalizer> logger,
        INameTransformer nameTransformer)
    {
        _logger = logger;
        _nameTransformer = nameTransformer;
        _normalizedNames = knowledgeAggregator.GetAggregatedKnowledge();
        _logger.LogInformation("normalizer ready");
    }

    public int NameCount => _normalizedNames.Count;

    public QueryResult? Query(string queryName, QueryOptions? options = null)
    {
        options ??= new QueryOptions();
        var transformedName = _nameTransformer.Transform(queryName);

        if ((options.MatchType & MatchType.Exact) != 0)
        {
            var result = FindExactMatch(transformedName);
            if (result != null)
                return result;
        }

        if ((options.MatchType & MatchType.Substring) != 0)
        {
            var result = FindSubstringMatch(queryName, options.NGrams);
            if (result != null)
                return result;
        }

        if ((options.MatchType & MatchType.Partial) != 0)
        {
            var result = FindPartialMatch(transformedName, options.Decimals, options.Threshold);
            if (result != null)
                return result;
        }

        _logger.LogDebug("no result for {}", queryName);
        return null;
    }

    private QueryResult? FindPartialMatch(string transformedName, int decimals, decimal threshold)
    {
        var minDist = decimal.MaxValue;
        List<QueryResultEntry> minDistValues = new();

        var distanceObj = new Levenshtein(transformedName);
        foreach (var (otherName, otherIds) in _normalizedNames)
        {
            var distance = distanceObj.DistanceFrom(otherName)
                           / (decimal)Math.Max(transformedName.Length, otherName.Length);
            distance = Math.Round(distance, decimals);

            if (distance < minDist)
            {
                minDistValues.Clear();
                minDist = distance;
            }

            if (distance == minDist)
                minDistValues.Add(new QueryResultEntry(otherName, otherIds));
        }

        if (minDist > threshold)
            return null;

        return new QueryResult(
            Type: MatchType.Partial,
            Entries: minDistValues,
            EditDistance: minDist
        );
    }

    private QueryResult? FindSubstringMatch(string name, int nGrams)
    {
        var substrings = name.Split(" ").ToList();
        for (var i = 2; i < nGrams + 1; i++)
        {
            var grams = Helpers.Ngrams(substrings, i)
                .Select(grams => string.Join(" ", grams));
            substrings.AddRange(grams);
        }

        var relevantSubstrings = substrings
            .Select(_nameTransformer.Transform)
            .Where(transformed => transformed.Length != 0);

        List<QueryResultEntry> matches = new();
        foreach (var transformed in relevantSubstrings)
        {
            if (_normalizedNames.TryGetValue(transformed, out var foundIds))
                matches.Add(new QueryResultEntry(transformed, foundIds));
        }

        return matches.Count > 0
            ? new QueryResult(Type: MatchType.Substring, Entries: matches)
            : null;
    }

    private QueryResult? FindExactMatch(string transformedName)
    {
        if (!_normalizedNames.TryGetValue(transformedName, out var foundIds))
            return null;

        return new QueryResult(Type: MatchType.Exact, Entries:
        [
            new QueryResultEntry(transformedName, foundIds)
        ]);
    }
}
