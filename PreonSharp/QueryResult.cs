namespace PreonSharp;

public record class QueryResult(
    MatchType Type,
    IReadOnlyList<QueryResultEntry> Entries,
    decimal? EditDistance = null);

public record struct QueryResultEntry(
    string Name,
    IReadOnlySet<NamespacedId> Ids
);
