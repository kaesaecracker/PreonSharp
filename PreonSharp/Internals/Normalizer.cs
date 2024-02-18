using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PreonSharp.Internals;

internal sealed class Normalizer : INormalizer
{
    private readonly FrozenDictionary<string, FrozenSet<string>> _normalizedNames;
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

    public Task<QueryResult> QueryAsync(string queryName, CancellationToken? token = null) => Task.Run(() =>
    {
        var stopWatch = Stopwatch.StartNew();

        var transformedName = _nameTransformer.Transform(queryName);

        foreach (var strategy in _matchStrategies)
        {
            token?.ThrowIfCancellationRequested();

            var result = strategy.FindMatch(transformedName, _normalizedNames, token);
            if (result != null)
                return Task.FromResult(result with { ExecutionTime = stopWatch.Elapsed });
        }

        _logger.LogDebug("no result for {}", queryName);
        return Task.FromResult(
            new QueryResult(MatchType.None, ReadOnlyCollection<QueryResultEntry>.Empty, stopWatch.Elapsed));
    });
}