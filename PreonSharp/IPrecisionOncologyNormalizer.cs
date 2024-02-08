namespace PreonSharp;

public interface IPrecisionOncologyNormalizer
{
    int NameCount { get; }
    QueryResult? Query(string queryName, QueryOptions? options = null);
}
