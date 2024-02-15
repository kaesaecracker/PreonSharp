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

    private void AggregateFrom(IKnowledgeProvider provider,
        ConcurrentDictionary<string, ConcurrentBag<NamespacedId>> wipNames)
    {
        var nameSpace = provider.SourceName;
        foreach (var (name, id) in provider.GetNameIdPairs())
        {
            var transformedName = _nameTransformer.Transform(name);
            var namespacedId = new NamespacedId(nameSpace, id);

            wipNames.GetOrAdd(transformedName, _ => [])
                .Add(namespacedId);
        }
    }

    private async Task<FrozenDictionary<string, FrozenSet<NamespacedId>>> BuildKnowledge()
    {
        _logger.LogInformation("Aggregating knowledge from {} providers", _knowledgeProviders.Length);
        ConcurrentDictionary<string, ConcurrentBag<NamespacedId>> wipNames = new();

        var buildTasks = _knowledgeProviders.Select(p => Task.Run(() => AggregateFrom(p, wipNames)));
        var buildDoneTask = Task.WhenAll(buildTasks);

        var nextOutput = 0;
        const int delta = 100000;
        while (!buildDoneTask.IsCompleted)
        {
            await Task.Delay(10);
            var count = wipNames.Count;
            if (count <= nextOutput)
                continue;

            _logger.LogDebug("loaded {} names so far", count);
            nextOutput = count + delta;
        }

        await buildDoneTask;
        _logger.LogInformation("loaded {} names, freezing now", wipNames.Count);
        return wipNames.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToFrozenSet());
    }
}
