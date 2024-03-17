namespace Taxonomy.Indexes;

public sealed class NameIndex(IEntityProvider entityProvider) : Index(entityProvider)
{
    protected override IEnumerable<string> Selector(Entity entity)
    {
        return entity.Names.Select(t => t.Value);
    }
}