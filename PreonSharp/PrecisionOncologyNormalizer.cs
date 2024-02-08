using System.Collections.Frozen;
using Fastenshtein;
using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp;

[Flags]
public enum MatchType : byte
{
    Partial = 0b0001,
    Exact = 0b0010,
    Substring = 0b0100,
    All = Partial | Exact | Substring,
}

public record class QueryOptions(
    MatchType MatchType = MatchType.All,
    decimal Threshold = 0.2m,
    int NGrams = 1,
    int Decimals = 3
);

public record class QueryResult(
    MatchType Type,
    IReadOnlyList<QueryResultEntry> Entries,
    decimal? EditDistance = null);

public record class QueryResultEntry(
    string Name,
    IReadOnlySet<string> Ids
);

public class PrecisionOncologyNormalizerBuilder(IServiceProvider services)
{
    private ILogger _logger = services.GetRequiredService<ILogger<PrecisionOncologyNormalizerBuilder>>();
    private readonly Dictionary<string, HashSet<string>> _normalizedNames = new();

    public PrecisionOncologyNormalizerBuilder Fit(string[] names, string[] ids)
    {
        if (names.Length != ids.Length)
            throw new ArgumentException($"length of {nameof(names)} and {nameof(ids)} must be equal");

        foreach (var (name, id) in names.Zip(ids))
            Fit(name, id);

        return this;
    }

    public PrecisionOncologyNormalizerBuilder Fit(string name, string id)
    {
        var transformedName = Helpers.TransformName(name);
        _logger.LogTrace("fitting name {} to id {} with transformation {}", name, id, transformedName);
        if (_normalizedNames.TryGetValue(transformedName, out var existingIds))
            existingIds.Add(id);
        else
            _normalizedNames.Add(transformedName, [id]);
        return this;
    }

    public PrecisionOncologyNormalizer Build()
    {
        var normalizerLogger = services.GetRequiredService<ILogger<PrecisionOncologyNormalizer>>();
        var readOnlyNames = _normalizedNames
            .Select(kvp => new KeyValuePair<string, IReadOnlySet<string>>(kvp.Key, kvp.Value.ToFrozenSet()))
            .ToFrozenDictionary();
        return new PrecisionOncologyNormalizer(readOnlyNames, normalizerLogger);
    }
}

public class PrecisionOncologyNormalizer(
    IReadOnlyDictionary<string, IReadOnlySet<string>> normalizedNames,
    ILogger<PrecisionOncologyNormalizer> logger)
{
    public int NameCount => normalizedNames.Count;
    
    public QueryResult? Query(string queryName, QueryOptions? options = null)
    {
        options ??= new QueryOptions();
        var transformedName = Helpers.TransformName(queryName);

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

        return null;
    }

    private QueryResult? FindPartialMatch(string transformedName, int decimals, decimal threshold)
    {
        var minDist = decimal.MaxValue;
        List<QueryResultEntry> minDistValues = new();

        foreach (var (otherName, otherIds) in normalizedNames)
        {
            var distance = Levenshtein.Distance(transformedName, otherName)
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
        for (int i = 2; i < nGrams + 1; i++)
        {
            var grams = Helpers.Ngrams(substrings, i)
                .Select(grams => string.Join(" ", grams));
            substrings.AddRange(grams);
        }

        var relevantSubstrings = substrings
            .Select(Helpers.TransformName)
            .Where(transformed => transformed.Length != 0);

        List<QueryResultEntry> matches = new();
        foreach (var transformed in relevantSubstrings)
        {
            if (normalizedNames.TryGetValue(transformed, out var foundIds))
                matches.Add(new QueryResultEntry(transformed, foundIds));
        }

        return matches.Count > 0
            ? new QueryResult(Type: MatchType.Substring, Entries: matches)
            : null;
    }

    private QueryResult? FindExactMatch(string transformedName)
    {
        if (!normalizedNames.TryGetValue(transformedName, out var foundIds))
            return null;

        return new QueryResult(Type: MatchType.Exact, Entries:
        [
            new QueryResultEntry(transformedName, foundIds)
        ]);
    }
}