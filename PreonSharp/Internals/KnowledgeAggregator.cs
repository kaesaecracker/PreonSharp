using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace PreonSharp.Internals;

internal sealed class KnowledgeAggregator
{
    private readonly IKnowledgeProvider[] _knowledgeProviders;
    private readonly ILogger _logger;
    private readonly INameTransformer _nameTransformer;

    public KnowledgeAggregator(
        IEnumerable<IKnowledgeProvider> knowledgeProviders,
        IEnumerable<IKnowledgeProviderFactory> providerFactories,
        ILogger<KnowledgeAggregator> logger,
        INameTransformer nameTransformer)
    {
        _knowledgeProviders = providerFactories
            .SelectMany(f => f.GetKnowledgeProviders())
            .Concat(knowledgeProviders)
            .ToArray();
        _logger = logger;
        _nameTransformer = nameTransformer;
    }

    private void AggregateFrom(IKnowledgeProvider provider,
        ConcurrentDictionary<string, ConcurrentBag<string>> wipNames)
    {
        _logger.LogInformation("starting read from {}", provider);
        foreach (var (name, id) in provider.GetNameIdPairs())
        {
            var transformedName = _nameTransformer.Transform(name);
            wipNames.GetOrAdd(transformedName, _ => [])
                .Add(id);
        }

        _logger.LogInformation("done read from {}", provider);
    }

    public async Task<FrozenDictionary<string, FrozenSet<string>>> BuildKnowledge()
    {
        _logger.LogInformation("Aggregating knowledge from {} providers", _knowledgeProviders.Length);
        ConcurrentDictionary<string, ConcurrentBag<string>> wipNames = new();

        var buildTasks = _knowledgeProviders.Select(p => Task.Run(() => AggregateFrom(p, wipNames)));
        var buildDoneTask = Task.WhenAll(buildTasks);

        var nextOutputTime = DateTime.Now;
        var nextOutput = 0;
        const int delta = 100000;
        while (!buildDoneTask.IsCompleted)
        {
            await Task.Delay(10);
            if (wipNames.Count <= nextOutput && DateTime.Now <= nextOutputTime)
                continue;

            var count = wipNames.Count;
            nextOutput = count + delta;
            nextOutputTime = DateTime.Now.AddSeconds(1);
            _logger.LogDebug("loaded {:n0} names so far", count);
        }

        await buildDoneTask;

        var idCount = wipNames.Sum(kvp => kvp.Value.Count);
        _logger.LogInformation("loaded {:n0} names with {:n0} ids, freezing now", wipNames.Count, idCount);
        return wipNames.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToFrozenSet());
    }
}