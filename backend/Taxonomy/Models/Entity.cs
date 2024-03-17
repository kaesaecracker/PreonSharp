namespace Taxonomy.Models;

public sealed record Entity(
    Guid Id,
    ISet<EntitySource> Sources,
    ISet<EntityTag> Names,
    ISet<EntityTag> Tags,
    ISet<EntityRelation> Relations
);