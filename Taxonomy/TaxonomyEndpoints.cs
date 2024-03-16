using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Taxonomy;

public class TaxonomyEndpoints(EntityProvider entityProvider)
{
    public RouteGroupBuilder Map(WebApplication app)
    {
        var mapGroup = app.MapGroup("/taxonomy");
        mapGroup.MapGet("/{guid:guid}", entityProvider.GetById);
        mapGroup.MapGet("/", (int count = 10) => entityProvider.GetFirst(count));
        return mapGroup;
    }
}