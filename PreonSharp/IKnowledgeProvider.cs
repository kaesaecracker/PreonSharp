namespace PreonSharp;

public interface IKnowledgeProvider
{
    string SourceName { get; }

    IEnumerable<(string, string)> GetNameIdPairs();
}
