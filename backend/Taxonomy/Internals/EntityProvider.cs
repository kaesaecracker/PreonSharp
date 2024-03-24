using System.Threading;
using Microsoft.Extensions.Hosting;

namespace Taxonomy.Internals;

internal sealed partial class EntityProvider(IEnumerable<IEntityLoader> loaders, ILogger<EntityProvider> logger, ProgressWatcherFactory watcherFactory)
    : BackgroundService, IEntityProvider
{
    private readonly Dictionary<Guid, Entity> _entities = new();
    private readonly Dictionary<string, IdNamespace> _idNamespaces = new();
    private readonly Dictionary<IdNamespace, Dictionary<string, Entity>> _entitiesBySourceId = new();

    private readonly TaskCompletionSource _startCompletion = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var builder = new EntityProviderBuilder(this);
        foreach (var loader in loaders)
        {
            logger.LogInformation("loading from {}", loader.GetType().Name);
            await loader.Load(builder);
        }

        logger.LogInformation("done loading");
        _startCompletion.SetResult();
    }

    public async Task<Entity?> GetById(Guid id)
    {
        await _startCompletion.Task;
        return _entities.GetValueOrDefault(id);
    }

    public Entity? GetByNamespacedId(string idNamespace, string id)
    {
        if (!_idNamespaces.TryGetValue(idNamespace, out var idNamespaceObj))
            return null;
        return   _entitiesBySourceId[idNamespaceObj].GetValueOrDefault(id);
    }

    public async Task<IEnumerable<Entity>> GetFirst(int count)
    {
        await _startCompletion.Task;
        return _entities.Values.Take(count);
    }

    public Task Started => _startCompletion.Task;

    public IEnumerable<Entity> All => _entities.Values;
}