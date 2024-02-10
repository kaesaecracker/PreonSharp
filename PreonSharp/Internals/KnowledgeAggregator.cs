using System.Collections.Frozen;

namespace PreonSharp.Internals;

internal sealed class KnowledgeAggregator
{
    private readonly IKnowledgeProvider[] _knowledgeProviders;
    private readonly ILogger<KnowledgeAggregator> _logger;
    private readonly INameTransformer _nameTransformer;

    public KnowledgeAggregator(IEnumerable<IKnowledgeProvider> knowledgeProviders,
        ILogger<KnowledgeAggregator> logger,
        INameTransformer nameTransformer)
    {
        _knowledgeProviders = knowledgeProviders.ToArray();
        _logger = logger;
        _nameTransformer = nameTransformer;
    }

    public FrozenDictionary<string,FrozenSet<NamespacedId>> GetAggregatedKnowledge()
    {
        _logger.LogInformation("Aggregating knowledge from {} providers", _knowledgeProviders.Length);
        Dictionary<string, HashSet<NamespacedId>> wipNames = new();
        foreach (var provider in _knowledgeProviders)
        {
            var namespacedId = new NamespacedId(provider.SourceName, string.Empty);
            foreach (var (name, id) in provider.GetNameIdPairs())
            {
                var transformedName = _nameTransformer.Transform(name);
                namespacedId = namespacedId with { Id = id };
                if (wipNames.TryGetValue(transformedName, out var existingIds))
                    existingIds.Add(namespacedId);
                else
                    wipNames.Add(transformedName, [namespacedId]);
            }

            _logger.LogInformation("loaded {} names so far", wipNames.Count);
        }

        _logger.LogInformation("freezing knowledge data");
        return wipNames.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToFrozenSet());
    }
}
