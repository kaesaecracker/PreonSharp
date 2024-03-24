using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace Taxonomy.Internals;

internal sealed partial class EntityProvider(
    IEnumerable<IEntityLoader> loaders,
    ILogger<IEntityProvider> logger,
    ILogger<IEntityProviderBuilder> builderLogger,
    ProgressWatcherFactory watcherFactory
) : BackgroundService, IEntityProvider
{
    private readonly SemaphoreSlim _entityCreationMutex = new(1, 1);
    private readonly ConcurrentDictionary<Guid, Entity> _entities = new();
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Entity>> _entitiesBySourceId = new();

    private readonly TaskCompletionSource _startCompletion = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var builder = new EntityProviderBuilder(this, builderLogger);
        var loaderTasks = Task.WhenAll(loaders.Select(l => l.Load(builder)));
        
        await watcherFactory.Indeterminate(loaderTasks, logger, () => _entities.Count);

        logger.LogInformation("done loading");
        _startCompletion.SetResult();
    }

    public async Task<Entity?> GetById(Guid id)
    {
        await _startCompletion.Task;
        return _entities.GetValueOrDefault(id);
    }

    public Entity? GetByNamespacedId(string idNamespace, string id)
        => _entitiesBySourceId[idNamespace].GetValueOrDefault(id);

    public async Task<IEnumerable<Entity>> GetFirst(int count)
    {
        await _startCompletion.Task;
        return _entities.Values.Take(count);
    }

    public Task Started => _startCompletion.Task;

    public IEnumerable<Entity> All
    {
        get
        {
            Trace.Assert(_startCompletion.Task.IsCompleted);
            return _entities.Values;
        }
    }
}