namespace PreonSharp.Internals;

internal class SinglePairKnowledge(string name, string id) : IKnowledgeProvider
{
    public IEnumerable<(string, string)> GetNameIdPairs()
    {
        yield return (name, id);
    }
}
