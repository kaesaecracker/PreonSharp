using System.Collections.Frozen;

namespace Loaders.Ncbi;

public sealed record TaxonomyEntity(
    ulong TaxonomyId,
    FrozenSet<TaxonomyTag> Tags,
    ulong? Parent
);

public sealed record TaxonomyTag(string Kind, string Value);