namespace PreonSharp;

public interface IKnowledgeProvider
{
    IEnumerable<(string, string)> GetNameIdPairs();
}
