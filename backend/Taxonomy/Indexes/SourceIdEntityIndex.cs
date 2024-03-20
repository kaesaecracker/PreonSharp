using Microsoft.Extensions.Logging;
using Normalizer;

namespace Taxonomy.Indexes;

public sealed class SourceIdEntityIndex(
    IEntityProvider entityProvider,
    INameTransformer nameTransformer,
    ILogger<SourceIdEntityIndex> logger)
    : EntityIndex(entityProvider, nameTransformer, logger)
{
    protected override IEnumerable<string> Selector(Entity entity)
    {
        return entity.Sources.Select(s => s.SourceId);
    }
}