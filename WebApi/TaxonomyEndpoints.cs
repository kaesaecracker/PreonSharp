using Taxonomy;

namespace WebApi;

internal sealed class TaxonomyEndpoints(IEntityProvider entityProvider)
{
    public RouteGroupBuilder Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", entityProvider.GetById);
        group.MapGet("/", (int count = 10) => entityProvider.GetFirst(count));
        return group;
    }
}