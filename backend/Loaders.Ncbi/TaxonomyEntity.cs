using Taxonomy.Models;

namespace Loaders.Ncbi;

public sealed record TaxonomyEntity(
    ulong TaxonomyId,
    ISet<EntityTag> Tags,
    ulong? Parent
);