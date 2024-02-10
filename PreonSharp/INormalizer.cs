namespace PreonSharp;

public interface INormalizer
{
    int NameCount { get; }
    QueryResult? Query(string queryName, QueryOptions? options = null);
}
