using System.Collections.Concurrent;
using System.Diagnostics;

namespace Taxonomy.Internals;

internal sealed partial class EntityProvider
{
    private sealed class EntityProviderBuilder(EntityProvider target, ILogger<IEntityProviderBuilder> logger)
        : IEntityProviderBuilder
    {
        public Task<Guid> ReferenceEntity(string idNamespace, string id)
            => AddEntity(idNamespace, id, new HashSet<EntityTag>(), new HashSet<EntityTag>());

        private Entity? FindSourceId(string idNamespace, string id)
        {
            target._entitiesBySourceId
                .GetOrAdd(idNamespace, ns => new ConcurrentDictionary<string, Entity>())
                .TryGetValue(id, out var entity);
            return entity;
        }

        public async Task<Guid> AddEntity(string idNamespace, string id, IEnumerable<EntityTag> names,
            IEnumerable<EntityTag> tags)
        {
            var entity = FindSourceId(idNamespace, id)
                         ?? await AddEntityLocked(idNamespace, id);

            foreach (var tag in names)
                entity.Names.Add(tag);
            foreach (var tag in tags)
                entity.Tags.Add(tag);
            
            Trace.Assert(entity != null);
            if (logger.IsEnabled(LogLevel.Trace))
                logger.LogTrace("added entity {} from source {} with id {}", entity.Id, idNamespace, id);
            return entity.Id;
        }

        private async Task<Entity> AddEntityLocked(string idNamespace, string id)
        {
            await target._entityCreationMutex.WaitAsync();
            try
            {
                var entity = FindSourceId(idNamespace, id);
                if (entity != null)
                    return entity;

                var guid = Guid.NewGuid();
                ConcurrentBag<EntitySource> sources = [new EntitySource(idNamespace, id)];
                entity = new Entity(guid, sources, [], [], []);

                var success = target._entities.TryAdd(guid, entity)
                              && target._entitiesBySourceId[idNamespace].TryAdd(id, entity);
                Trace.Assert(success);

                return entity;
            }
            finally
            {
                target._entityCreationMutex.Release();
            }
        }

        public Task AddRelation(string fromKind, string toKind, Guid fromId, Guid toId)
        {
            target._entities[fromId].Relations.Add(new EntityRelation(toKind, toId));
            target._entities[toId].Relations.Add(new EntityRelation(fromKind, fromId));
            return Task.CompletedTask;
        }
    }
}