using Taxonomy;

namespace WebApi;

internal sealed class TaxonomyEndpoints(IEntityProvider entityProvider, IEntitySearcher entitySearcher)
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", entityProvider.GetById);
        group.MapGet("/", (int count = 10) => entityProvider.GetFirst(count));
        group.MapGet("/search", entitySearcher.GetExactMatches);
    }
}