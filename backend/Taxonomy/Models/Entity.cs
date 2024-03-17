namespace Taxonomy.Models;

public sealed record Entity(
    Guid Id,
    ISet<EntitySource> Sources,
    ISet<EntityTag> Tags,
    ISet<EntityRelation> Relations
);