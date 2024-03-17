using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Threading.Channels;

namespace Normalizer.Internals;

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
            .SelectMany(f => f.BuildConfiguredKnowledgeProviders())
            .Concat(knowledgeProviders)
            .ToArray();
        _logger = logger;
        _nameTransformer = nameTransformer;
    }

    // Test with 5 producers
    // 8 consumers, 1000 elements, AllowSynchronousContinuations=true -> 3:45
    // 4 consumers, 1000 elements, AllowSynchronousContinuations=true -> 2:47
    // 4 consumers, 100 elements, AllowSynchronousContinuations=true -> 3:21
    // 4 consumers, 100 elements, AllowSynchronousContinuations=false -> 3:16
    // 4 consumers, 1000 elements, AllowSynchronousContinuations=false -> 3:18
    // 2 consumers, 1000 elements, AllowSynchronousContinuations=false -> 2:10
    // 2 consumers, 100 elements, AllowSynchronousContinuations=false -> 2:17
    // 1 consumers, 1000 elements, AllowSynchronousContinuations=false -> 2:36
    // 3 consumers, 3000 elements, AllowSynchronousContinuations=false -> 2:41
    // 3 consumers, 500 elements, AllowSynchronousContinuations=false -> 2:48
    // 1 consumers, 1000 elements, AllowSynchronousContinuations=false, SingleReader = true -> 2:37
    // 1 consumers, 2000 elements, AllowSynchronousContinuations=false -> 2:37, 2:08
    // 2 consumers, 500 elements, AllowSynchronousContinuations=false -> 2:10
    // 2 consumers, 500 elements, AllowSynchronousContinuations=true -> 2:35
    // 2 consumers, 200 elements, AllowSynchronousContinuations=false -> 2:22

    private async Task WriteChannelToDict(ChannelReader<KnowledgeDataPoint> channel,
        ConcurrentDictionary<string, ConcurrentBag<string>> wipNames)
    {
        _logger.LogInformation("starting reading");
        await foreach (var k in channel.ReadAllAsync())
        {
            var transformedName = string.Intern(_nameTransformer.Transform(k.Name));
            wipNames.GetOrAdd(transformedName, _ => [])
                .Add(string.Intern(k.Id));
        }

        _logger.LogInformation("done reading");
    }

    public async Task<FrozenDictionary<string, FrozenSet<string>>> BuildKnowledge()
    {
        _logger.LogInformation("Aggregating knowledge from {} providers", _knowledgeProviders.Length);
        var sw = Stopwatch.StartNew();

        var inChannel = Channel.CreateBounded<KnowledgeDataPoint>(new BoundedChannelOptions(500)
        {
            SingleWriter = false,
            SingleReader = false,
            AllowSynchronousContinuations = false,
            FullMode = BoundedChannelFullMode.Wait,
        });

        var buildTasks = _knowledgeProviders.Select(p => p.WriteKnowledgeTo(inChannel));
        var buildDoneTask = Task.WhenAll(buildTasks).ContinueWith(_ =>
        {
            inChannel.Writer.Complete();
            _logger.LogDebug("marking writer as completed");
        });
        _logger.LogDebug("producer tasks created after {}", sw.Elapsed);

        ConcurrentDictionary<string, ConcurrentBag<string>> wipNames = new();
        var writeTask = Task.WhenAll(WriteChannelToDict(inChannel, wipNames), WriteChannelToDict(inChannel, wipNames));

        _logger.LogDebug("consumer tasks created after {}", sw.Elapsed);

        await WaitForCompletionWithProgress(Task.WhenAll(buildDoneTask, writeTask), wipNames);

        _logger.LogDebug("done reading after {}", sw.Elapsed);

        var idCount = wipNames.Sum(kvp => kvp.Value.Count);
        _logger.LogInformation("loaded {:n0} names with {:n0} ids, freezing now", wipNames.Count, idCount);
        var frozenDictionary = wipNames.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToFrozenSet());

        _logger.LogDebug("done freezing after {}", sw.Elapsed);
        return frozenDictionary;
    }

    private async Task WaitForCompletionWithProgress(Task task,
        ConcurrentDictionary<string, ConcurrentBag<string>> wipNames)
    {
        var nextOutputTime = DateTime.Now;
        var nextOutput = 0;

        const int delta = 100000;
        while (!task.IsCompleted)
        {
            await Task.Delay(50);
            if (wipNames.Count <= nextOutput && DateTime.Now <= nextOutputTime)
                continue;

            var count = wipNames.Count;
            nextOutput = count + delta;
            nextOutputTime = DateTime.Now.AddSeconds(1);
            _logger.LogDebug("loaded {:n0} names so far", count);
        }

        await task;
    }
}
