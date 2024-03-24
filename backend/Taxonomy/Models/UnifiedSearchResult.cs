namespace Taxonomy.Models;

public sealed record UnifiedSearchResult(
    string OriginalQuery,
    string TransformedQuery,
    TimeSpan QueryTime,
    UnifiedSearchResultKind Kind,
    IEnumerable<TextMatch> Matches
);