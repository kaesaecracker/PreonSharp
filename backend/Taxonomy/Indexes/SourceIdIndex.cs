using Normalizer;

namespace Taxonomy.Indexes;

public sealed class SourceIdIndex(IEntityProvider entityProvider, INameTransformer nameTransformer)
    : Index(entityProvider, nameTransformer)
{
    protected override IEnumerable<string> Selector(Entity entity)
    {
        return entity.Sources.Select(s => s.SourceId);
    }
}