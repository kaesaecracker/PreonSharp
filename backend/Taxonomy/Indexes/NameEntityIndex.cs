using Normalizer;

namespace Taxonomy.Indexes;

public sealed class NameEntityIndex(IEntityProvider entityProvider, INameTransformer nameTransformer)
    : EntityIndex(entityProvider, nameTransformer)
{
    protected override IEnumerable<string> Selector(Entity entity)
    {
        return entity.Names.Select(t => t.Value);
    }
}