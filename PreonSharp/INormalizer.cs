namespace PreonSharp;

public interface INormalizer
{
    Task<QueryResult?> QueryAsync(string queryName);
}
