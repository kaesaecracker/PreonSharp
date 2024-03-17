namespace Taxonomy;

public interface IEntityProviderBuilder
{
    Guid AddEntity(EntitySource source, ISet<EntityTag> names, ISet<EntityTag> tags);

    /// <summary>
    /// Example: "child", "parent", idOfChild, idOfParent
    /// </summary>
    void AddRelation(string fromKind, string toKind, Guid fromId, Guid toId);
}