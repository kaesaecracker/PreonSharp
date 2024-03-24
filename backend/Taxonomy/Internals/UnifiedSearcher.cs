using System.Diagnostics;

namespace Taxonomy.Internals;

internal sealed class UnifiedSearcher(
    INameTransformer nameTransformer,
    IEntitySearcher searcher,
    IEntityProvider provider
) : IUnifiedSearcher
{
    public async Task<UnifiedSearchResult> UnifiedSearch(string text)
    {
        var stopWatch = Stopwatch.StartNew();

        if (Guid.TryParse(text, out var guid))
        {
            var entity = await provider.GetById(guid);
            if (entity != null)
            {
                var guidStr = guid.ToString();
                var match = new TextMatch(guidStr, new HashSet<Guid>([entity.Id]));
                return new UnifiedSearchResult(text, guidStr, stopWatch.Elapsed,
                    UnifiedSearchResultKind.InternalId, [match]);
            }
        }

        var transformed = nameTransformer.Transform(text);

        var exact = await searcher.GetExactMatches(text);
        if (exact.EntityIds.Count > 0)
        {
            return new UnifiedSearchResult(text, transformed, stopWatch.Elapsed,
                UnifiedSearchResultKind.Exact, [exact]);
        }

        var closest = await searcher.GetClosestNames(text);
        if (closest.Count > 0)
        {
            return new UnifiedSearchResult(text, transformed, stopWatch.Elapsed,
                UnifiedSearchResultKind.ClosestName, closest);
        }

        return new UnifiedSearchResult(text, transformed, stopWatch.Elapsed, UnifiedSearchResultKind.None, []);
    }
}