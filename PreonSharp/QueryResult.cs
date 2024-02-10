namespace PreonSharp;

public record class QueryResult(
    MatchType Type,
    IReadOnlyList<QueryResultEntry> Entries,
    decimal? EditDistance = null);

public record class QueryResultEntry(
    string Name,
    IReadOnlySet<NamespacedId> Ids
);
