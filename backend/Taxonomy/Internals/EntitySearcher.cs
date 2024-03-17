using System.Threading;
using Microsoft.Extensions.Hosting;
using Normalizer;
using Taxonomy.Indexes;

namespace Taxonomy.Internals;

internal sealed class EntitySearcher : BackgroundService, IEntitySearcher
{
    private readonly TaskCompletionSource _startCompletion = new();
    private readonly EntityIndex _nameIndex;
    private readonly EntityIndex[] _indices;
    private readonly IEntityProvider _entityProvider;
    private readonly INameTransformer _nameTransformer;

    public EntitySearcher(IEntityProvider entityProvider, INameTransformer nameTransformer)
    {
        _entityProvider = entityProvider;
        _nameTransformer = nameTransformer;
        _nameIndex = new NameEntityIndex(entityProvider, nameTransformer);
        _indices =
        [
            _nameIndex,
            new SourceIdEntityIndex(entityProvider, nameTransformer)
        ];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _entityProvider.Started;
        await Task.WhenAll(_indices.Select(i => i.BuildAsync()));
        _startCompletion.SetResult();
    }

    public Task Started => _startCompletion.Task;

    public async Task<TextMatch> GetExactMatches(string text)
    {
        text = _nameTransformer.Transform(text);
        await Started;

        var set = _indices.Select(i => i.GetExactMatches(text))
            .Where(s => s != null)
            .SelectMany(s => s! /* checked by where */)
            .ToHashSet();
        return new TextMatch(text, set);
    }

    public async Task<IEnumerable<TextMatch>> GetClosestNames(string text)
    {
        text = _nameTransformer.Transform(text);
        await Started;
        return _nameIndex.GetClosestMatches(text);
    }
}