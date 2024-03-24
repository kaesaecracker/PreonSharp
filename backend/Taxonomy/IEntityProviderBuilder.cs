namespace Taxonomy;

public interface IEntityProviderBuilder
{
    Task<Guid> AddEntity(string idNamespace, string id, IEnumerable<EntityTag> names, IEnumerable<EntityTag> tags);

    /// <summary>
    /// Example: "child", "parent", idOfChild, idOfParent
    /// </summary>
    Task AddRelation(string fromKind, string toKind, Guid fromId, Guid toId);

    Task<Guid> ReferenceEntity(string idNamespace, string id);
}