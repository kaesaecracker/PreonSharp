using Normalizer;

namespace Taxonomy.Indexes;

public sealed class SourceIdEntityIndex(IEntityProvider entityProvider, INameTransformer nameTransformer)
    : EntityIndex(entityProvider, nameTransformer)
{
    protected override IEnumerable<string> Selector(Entity entity)
    {
        return entity.Sources.Select(s => s.SourceId);
    }
}