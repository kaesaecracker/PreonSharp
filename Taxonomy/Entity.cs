namespace Taxonomy;

public sealed record Entity(
    Guid Id,
    ISet<EntitySource> Sources,
    ISet<EntityTag> Tags,
    ISet<EntityRelation> Relations
);

public sealed record EntityTag(string Kind, string Value);

public sealed record EntityRelation(string Kind, Guid Other);

public sealed record EntitySource(Source Source, string SourceId);

public sealed record Source(string Name);