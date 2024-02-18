using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace PreonSharp.Internals;

internal sealed class KnowledgeAggregator
{
    private readonly IKnowledgeProvider[] _knowledgeProviders;
    private readonly ILogger<KnowledgeAggregator> _logger;
    private readonly INameTransformer _nameTransformer;
    private readonly Task<FrozenDictionary<string, FrozenSet<string>>> _knowledgeTask;

    public KnowledgeAggregator(IEnumerable<IKnowledgeProvider> knowledgeProviders,
        ILogger<KnowledgeAggregator> logger,
        INameTransformer nameTransformer)
    {
        _knowledgeProviders = knowledgeProviders.ToArray();
        _logger = logger;
        _nameTransformer = nameTransformer;
        _knowledgeTask = BuildKnowledge();
    }

    public Task<FrozenDictionary<string, FrozenSet<string>>> GetAggregatedKnowledge() => _knowledgeTask;

    private void AggregateFrom(IKnowledgeProvider provider,
        ConcurrentDictionary<string, ConcurrentBag<string>> wipNames)
    {
        foreach (var (name, id) in provider.GetNameIdPairs())
        {
            var transformedName = _nameTransformer.Transform(name);
            wipNames.GetOrAdd(transformedName, _ => [])
                .Add(id);
        }
    }

    private async Task<FrozenDictionary<string, FrozenSet<string>>> BuildKnowledge()
    {
        _logger.LogInformation("Aggregating knowledge from {} providers", _knowledgeProviders.Length);
        ConcurrentDictionary<string, ConcurrentBag<string>> wipNames = new();

        var buildTasks = _knowledgeProviders.Select(p => Task.Run(() => AggregateFrom(p, wipNames)));
        var buildDoneTask = Task.WhenAll(buildTasks);

        var lastOutputTime = DateTime.Now;
        var nextOutput = 0;
        const int delta = 100000;
        while (!buildDoneTask.IsCompleted)
        {
            await Task.Delay(10);
            var count = wipNames.Count;

            var now = DateTime.Now;
            bool shouldPrint = count > nextOutput || now - lastOutputTime > TimeSpan.FromSeconds(1);
            if (!shouldPrint)
                continue;

            _logger.LogDebug("loaded {:n0} names so far", count);
            nextOutput = count + delta;
            lastOutputTime = now;
        }

        await buildDoneTask;

        var idCount = wipNames.Sum(kvp => kvp.Value.Count);
        _logger.LogInformation("loaded {:n0} names with {:n0} ids, freezing now", wipNames.Count, idCount);
        return wipNames.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToFrozenSet());
    }
}
