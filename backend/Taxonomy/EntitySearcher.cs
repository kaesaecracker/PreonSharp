using System.Threading;
using Microsoft.Extensions.Hosting;
using Taxonomy.Indexes;

namespace Taxonomy;

public sealed class EntitySearcher(IEntityProvider entityProvider) : BackgroundService, IStartAwaitable
{
    private readonly TaskCompletionSource _startCompletion = new();
    private readonly Index[] _indices = [new NameIndex(entityProvider), new NameIndex(entityProvider)];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await entityProvider.Started;
        await Task.WhenAll(_indices.Select(i => i.BuildAsync()));
        _startCompletion.SetResult();
    }

    public Task Started => _startCompletion.Task;

    public IEnumerable<Entity> GetExactMatches(string text)
    {
        return _indices.Select(i => i.GetExactMatch(text))
            .Where(s => s != null)
            .SelectMany(s => s! /* checked by where */)
            .Distinct();
    }
}