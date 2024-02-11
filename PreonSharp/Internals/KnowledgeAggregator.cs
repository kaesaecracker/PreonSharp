using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace PreonSharp.Internals;

internal sealed class KnowledgeAggregator
{
    private readonly IKnowledgeProvider[] _knowledgeProviders;
    private readonly ILogger<KnowledgeAggregator> _logger;
    private readonly INameTransformer _nameTransformer;
    private readonly Task<FrozenDictionary<string, FrozenSet<NamespacedId>>> _knowledgeTask;

    public KnowledgeAggregator(IEnumerable<IKnowledgeProvider> knowledgeProviders,
        ILogger<KnowledgeAggregator> logger,
        INameTransformer nameTransformer)
    {
        _knowledgeProviders = knowledgeProviders.ToArray();
        _logger = logger;
        _nameTransformer = nameTransformer;
        _knowledgeTask = BuildKnowledge();
    }

    public Task<FrozenDictionary<string, FrozenSet<NamespacedId>>> GetAggregatedKnowledge() => _knowledgeTask;

    private async Task AggregateFrom(IKnowledgeProvider provider,
        ConcurrentDictionary<string, ConcurrentBag<NamespacedId>> wipNames)
    {
        await foreach (var nameIdPair in provider.GetNameIdPairs())
        {
            var transformedName = _nameTransformer.Transform(nameIdPair.Item1);
            var namespacedId = new NamespacedId(string.Empty, nameIdPair.Item2);

            wipNames.GetOrAdd(transformedName, _ => [])
                .Add(namespacedId);
        }
    }

    private async Task<FrozenDictionary<string, FrozenSet<NamespacedId>>> BuildKnowledge()
    {
        _logger.LogInformation("Aggregating knowledge from {} providers", _knowledgeProviders.Length);
        ConcurrentDictionary<string, ConcurrentBag<NamespacedId>> wipNames = new();

        var done = false;
        var buildTask = Task.WhenAll(_knowledgeProviders.Select(p => AggregateFrom(p, wipNames)))
            .ContinueWith(_ => done = true);

        var nextOutput = 0;
        const int delta = 100000;
        while (!done)
        {
            await Task.Delay(10);
            var count = wipNames.Count;
            if (count <= nextOutput)
                continue;

            _logger.LogDebug("loaded {} names so far", count);
            nextOutput = count + delta;
        }

        await buildTask;

        _logger.LogInformation("loaded {} names, freezing now", wipNames.Count);
        return wipNames.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToFrozenSet());
    }
}
