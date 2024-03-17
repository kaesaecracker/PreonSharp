namespace Taxonomy.Indexes;

public sealed class SourceIdIndex(IEntityProvider entityProvider) :Index(entityProvider)
{
    protected override IEnumerable<string> Selector(Entity entity)
    {
        return entity.Sources.Select(s => s.SourceId);
    }
}