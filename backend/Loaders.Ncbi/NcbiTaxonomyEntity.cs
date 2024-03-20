using Taxonomy.Models;

namespace Loaders.Ncbi;

internal sealed record NcbiTaxonomyEntity(
    ulong TaxonomyId,
    ISet<EntityTag> Names,
    ISet<EntityTag> Tags,
    ulong? Parent
);