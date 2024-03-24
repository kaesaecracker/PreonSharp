namespace Taxonomy.Internals;

internal sealed partial class EntityProvider
{
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