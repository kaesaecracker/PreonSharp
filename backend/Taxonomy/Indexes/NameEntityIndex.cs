namespace Taxonomy.Indexes;

public sealed class NameEntityIndex(
    IEntityProvider entityProvider,
    INameTransformer nameTransformer,
    ILogger<NameEntityIndex> logger)
    : EntityIndex(entityProvider, nameTransformer, logger)
{
    protected override IEnumerable<string> Selector(Entity entity)
    {
        return entity.Names.Select(t => t.Value);
    }
}