namespace Taxonomy.Models;

public sealed record UnifiedSearchResult(
    string OriginalQuery,
    string TransformedQuery,
    UnifiedSearchResultKind Kind,
    IEnumerable<TextMatch> Matches
);