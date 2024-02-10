namespace PreonSharp.Internals;

internal class SeriesKnowledge(IEnumerable<string> names, IEnumerable<string> ids) : IKnowledgeProvider
{
    public IEnumerable<(string, string)> GetNameIdPairs() => names.Zip(ids);
}
