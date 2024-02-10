namespace PreonSharp.Internals;

internal sealed class SinglePairKnowledge(string name, string id) : IKnowledgeProvider
{
    public string SourceName => ToString()!;

    public IEnumerable<(string, string)> GetNameIdPairs()
    {
        yield return (name, id);
    }
}
