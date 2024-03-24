using Normalizer;

namespace Taxonomy.Internals;

internal sealed class UnifiedSearcher(
    INameTransformer nameTransformer,
    IEntitySearcher searcher,
    IEntityProvider provider
) : IUnifiedSearcher
{
    public async Task<UnifiedSearchResult> UnifiedSearch(string text)
    {
        if (Guid.TryParse(text, out var guid))
        {
            var entity = await provider.GetById(guid);
            if (entity != null)
            {
                var guidStr = guid.ToString();
                var match = new TextMatch(guidStr, new HashSet<Guid>([entity.Id]));
                return new UnifiedSearchResult(text, guidStr, UnifiedSearchResultKind.InternalId, [match]);
            }
        }

        var transformed = nameTransformer.Transform(text);

        var exact = await searcher.GetExactMatches(text);
        if (exact.EntityIds.Count > 0)
            return new UnifiedSearchResult(text, transformed, UnifiedSearchResultKind.Exact, [exact]);

        var closest = await searcher.GetClosestNames(text);
        if (closest.Count > 0)
            return new UnifiedSearchResult(text, transformed, UnifiedSearchResultKind.ClosestName, closest);

        return new UnifiedSearchResult(text, transformed, UnifiedSearchResultKind.None, []);
    }
}