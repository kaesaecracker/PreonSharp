namespace Taxonomy;

public interface IUnifiedSearcher
{
    Task<UnifiedSearchResult> UnifiedSearch(string text);
}