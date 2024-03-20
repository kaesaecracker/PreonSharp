namespace Taxonomy;

public interface IEntityProviderBuilder
{
    Guid AddEntity(IdNamespace idNamespace, string id, ISet<EntityTag> names, ISet<EntityTag> tags);

    /// <summary>
    /// Example: "child", "parent", idOfChild, idOfParent
    /// </summary>
    void AddRelation(string fromKind, string toKind, Guid fromId, Guid toId);

    IdNamespace AddIdNamespace(string name);

    Guid ReferenceEntity(IdNamespace idNamespace, string id);
}