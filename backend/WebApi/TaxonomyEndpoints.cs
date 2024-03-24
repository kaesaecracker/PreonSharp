using Taxonomy;

namespace WebApi;

internal sealed class TaxonomyEndpoints(IEntityProvider entityProvider, IEntitySearcher entitySearcher, IUnifiedSearcher searcher)
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", entityProvider.GetById);
        group.MapGet("/", (int count = 10) => entityProvider.GetFirst(count));

        group.MapGet("/find", searcher.UnifiedSearch);
        
        group.MapGet("/find-exact", entitySearcher.GetExactMatches);
        group.MapGet("/find-closest-name", entitySearcher.GetClosestNames);
        group.MapGet("/find-by-namespaced-id", entityProvider.GetByNamespacedId);
    }
}