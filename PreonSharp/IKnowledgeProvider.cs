namespace PreonSharp;

public interface IKnowledgeProvider
{
    string SourceName { get; }

    IAsyncEnumerable<(string, string)> GetNameIdPairs();
}
