using System.Threading;
using Microsoft.Extensions.Logging;

namespace Taxonomy;

internal sealed class EntityProvider(IEnumerable<IEntityLoader> loaders, ILogger<EntityProvider> logger) : IEntityProvider
{
    private readonly Dictionary<Guid, Entity> _entities = new();
    private readonly TaskCompletionSource _startCompletion = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var builder = new EntityProviderBuilder(this);
        foreach (var loader in loaders)
        {
            cancellationToken.ThrowIfCancellationRequested();
            logger.LogInformation("loading from {}", loader.GetType().Name);
            await loader.Load(builder);
        }

        logger.LogInformation("done loading");
        _startCompletion.SetResult();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task<Entity?> GetById(Guid id)
    {
        await _startCompletion.Task;
        return _entities.GetValueOrDefault(id);
    }

    public async Task<IEnumerable<Entity>> GetFirst(int count)
    {
        await _startCompletion.Task;
        return _entities.Values.Take(count);
    }

    private sealed class EntityProviderBuilder(EntityProvider target) : IEntityProviderBuilder
    {
        public Guid AddEntity(EntitySource source, ISet<EntityTag> tags)
        {
            var guid = Guid.NewGuid();
            var entity = new Entity(guid, new HashSet<EntitySource>([source]), tags, new HashSet<EntityRelation>());
            target._entities.Add(guid, entity);
            return guid;
        }

        public void AddRelation(string fromKind, string toKind, Guid fromId, Guid toId)
        {
            target._entities[fromId].Relations.Add(new EntityRelation(string.Intern(toKind), toId));
            target._entities[toId].Relations.Add(new EntityRelation(string.Intern(fromKind), fromId));
        }
    }
}