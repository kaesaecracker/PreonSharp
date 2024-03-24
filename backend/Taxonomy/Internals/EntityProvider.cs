using System.Threading;
using Microsoft.Extensions.Hosting;

namespace Taxonomy.Internals;

internal sealed class EntityProvider(IEnumerable<IEntityLoader> loaders, ILogger<EntityProvider> logger)
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

    private sealed class EntityProviderBuilder(EntityProvider target) : IEntityProviderBuilder
    {
        public IdNamespace AddIdNamespace(string name)
        {
            if (target._idNamespaces.TryGetValue(name, out var result))
                return result;

            result = new IdNamespace(name);
            target._idNamespaces.Add(name, result);
            target._entitiesBySourceId.Add(result, new Dictionary<string, Entity>());

            return result;
        }

        public Guid ReferenceEntity(IdNamespace idNamespace, string id)
        {
            if (target._entitiesBySourceId[idNamespace].TryGetValue(id, out var existingEntity))
                return existingEntity.Id;

            return AddEntity(idNamespace, id, new HashSet<EntityTag>(), new HashSet<EntityTag>());
        }

        public Guid AddEntity(IdNamespace idNamespace, string id, ISet<EntityTag> names, ISet<EntityTag> tags)
        {
            if (target._entitiesBySourceId[idNamespace].TryGetValue(id, out var existingEntity))
                return existingEntity.Id;

            var guid = Guid.NewGuid();
            HashSet<EntitySource> sources = [new EntitySource(idNamespace, id)];
            var entity = new Entity(guid, sources, names, tags, new HashSet<EntityRelation>());

            target._entities.Add(guid, entity);
            target._entitiesBySourceId[idNamespace].Add(id, entity);
            return guid;
        }

        public void AddRelation(string fromKind, string toKind, Guid fromId, Guid toId)
        {
            target._entities[fromId].Relations.Add(new EntityRelation(toKind, toId));
            target._entities[toId].Relations.Add(new EntityRelation(fromKind, fromId));
        }
    }
}