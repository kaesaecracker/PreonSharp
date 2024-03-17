using Normalizer;

namespace WebApi;

internal sealed class NormalizerEndpoints(INormalizer normalizer)
{
    public void Map(RouteGroupBuilder group)
    {
        group.MapGet("/query", (string s) => normalizer.QueryAsync(s));
        group.MapGet("/wait", normalizer.WaitForInitializationAsync);
    }
}