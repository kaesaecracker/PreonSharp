using System.Collections.Frozen;

namespace PreonSharp.Internals;

internal sealed class Normalizer : INormalizer
{
    private readonly FrozenDictionary<string, FrozenSet<NamespacedId>> _normalizedNames;
    private readonly ILogger<Normalizer> _logger;
    private readonly INameTransformer _nameTransformer;
    private readonly IMatchStrategy[] _matchStrategies;

    public Normalizer(KnowledgeAggregator knowledgeAggregator,
        ILogger<Normalizer> logger,
        INameTransformer nameTransformer,
        IEnumerable<IMatchStrategy> matchStrategies)
    {
        _logger = logger;
        _nameTransformer = nameTransformer;
        _normalizedNames = knowledgeAggregator.GetAggregatedKnowledge().Result;
        _logger.LogInformation("normalizer ready");
        _matchStrategies = matchStrategies.OrderBy(s => s.Cost).ToArray();
    }

    public int NameCount => _normalizedNames.Count;

    public async Task<QueryResult?> QueryAsync(string queryName)
    {
        var transformedName = _nameTransformer.Transform(queryName);

        foreach (var strategy in _matchStrategies)
        {
            var result = await strategy.FindMatchAsync(transformedName, _normalizedNames);
            if (result != null)
                return result;
        }

        _logger.LogDebug("no result for {}", queryName);
        return null;
    }
}
