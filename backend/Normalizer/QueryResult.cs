namespace Normalizer;

public record class QueryResult(
    MatchType Type,
    IReadOnlyList<QueryResultEntry> FoundIds,
    TimeSpan? ExecutionTime = null)
{
    public override string ToString()
    {
        return QueryResultJsonSerializerContext.Serialize(this);
    }
}

public record struct QueryResultEntry(
    string Name,
    IReadOnlySet<string> Ids
);