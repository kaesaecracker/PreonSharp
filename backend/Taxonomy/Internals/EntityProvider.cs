using System.Collections.Concurrent;
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
        var watcher = watcherFactory.Indeterminate(_startCompletion.Task, logger, () => _entities.Count);
        var builder = new EntityProviderBuilder(this, builderLogger);
        foreach (var loader in loaders)
        {
            logger.LogInformation("loading from {}", loader.GetType().Name);
            await loader.Load(builder);
        }

        logger.LogInformation("done loading");
        _startCompletion.SetResult();
        await watcher;
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

    public IEnumerable<Entity> All => _entities.Values;
}