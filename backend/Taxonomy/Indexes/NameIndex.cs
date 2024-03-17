using Normalizer;

namespace Taxonomy.Indexes;

public sealed class NameIndex(IEntityProvider entityProvider, INameTransformer nameTransformer)
    : Index(entityProvider, nameTransformer)
{
    protected override IEnumerable<string> Selector(Entity entity)
    {
        return entity.Names.Select(t => t.Value);
    }
}