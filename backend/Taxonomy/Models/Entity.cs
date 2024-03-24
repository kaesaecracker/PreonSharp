using System.Collections.Concurrent;

namespace Taxonomy.Models;

public sealed record Entity(
    Guid Id,
    ConcurrentBag<EntitySource> Sources,
    ConcurrentBag<EntityTag> Names,
    ConcurrentBag<EntityTag> Tags,
    ConcurrentBag<EntityRelation> Relations
);