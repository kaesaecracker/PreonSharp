using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace PreonSharp.Internals;

internal sealed class Normalizer : BackgroundService, INormalizer
{
    private FrozenDictionary<string, FrozenSet<string>>? _normalizedNames;
    private readonly KnowledgeAggregator _knowledgeAggregator;
    private readonly ILogger<Normalizer> _logger;
    private readonly INameTransformer _nameTransformer;
    private readonly IMatchStrategy[] _matchStrategies;
    private readonly TaskCompletionSource _initialisationCompletionSource = new();

    public Normalizer(KnowledgeAggregator knowledgeAggregator,
        ILogger<Normalizer> logger,
        INameTransformer nameTransformer,
        IEnumerable<IMatchStrategy> matchStrategies)
    {
        _knowledgeAggregator = knowledgeAggregator;
        _logger = logger;
        _nameTransformer = nameTransformer;
        _matchStrategies = matchStrategies.OrderBy(s => s.Cost).ToArray();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sw = Stopwatch.StartNew();
        _normalizedNames = await _knowledgeAggregator.BuildKnowledge();
        _initialisationCompletionSource.SetResult();
        _logger.LogInformation($"{nameof(Normalizer)} ready for queries after {{}}", sw.Elapsed);
    }

    public Task WaitForInitializationAsync() => _initialisationCompletionSource.Task;

    public async Task<QueryResult> QueryAsync(string queryName, CancellationToken? token = null)
    {
        await _initialisationCompletionSource.Task;
        Trace.Assert(_normalizedNames != null);

        var stopWatch = Stopwatch.StartNew();
        var transformedName = _nameTransformer.Transform(queryName);

        foreach (var strategy in _matchStrategies)
        {
            token?.ThrowIfCancellationRequested();

            var result = strategy.FindMatch(transformedName, _normalizedNames, token);
            if (result != null)
                return result with { ExecutionTime = stopWatch.Elapsed };
        }

        _logger.LogDebug("no result for {}", queryName);
        return new QueryResult(MatchType.None, ReadOnlyCollection<QueryResultEntry>.Empty, stopWatch.Elapsed);
    }
}
