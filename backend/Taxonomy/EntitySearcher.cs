using System.Threading;
using Microsoft.Extensions.Hosting;
using Normalizer;
using Taxonomy.Indexes;

namespace Taxonomy;

public sealed class EntitySearcher(IEntityProvider entityProvider, INameTransformer nameTransformer)
    : BackgroundService, IStartAwaitable
{
    private readonly TaskCompletionSource _startCompletion = new();

    private readonly Index[] _indices =
    [
        new NameIndex(entityProvider, nameTransformer),
        new NameIndex(entityProvider, nameTransformer)
    ];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await entityProvider.Started;
        await Task.WhenAll(_indices.Select(i => i.BuildAsync()));
        _startCompletion.SetResult();
    }

    public Task Started => _startCompletion.Task;

    public async Task<IEnumerable<Entity>> GetExactMatches(string text)
    {
        text = nameTransformer.Transform(text);
        await Started;
        
        return _indices.Select(i => i.GetExactMatch(text))
            .Where(s => s != null)
            .SelectMany(s => s! /* checked by where */)
            .Distinct();
    }
}