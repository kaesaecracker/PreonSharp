namespace PreonSharp.Internals;

internal sealed class SeriesKnowledge(string kbName, IEnumerable<string> names, IEnumerable<string> ids) : IKnowledgeProvider
{
    public string SourceName => kbName;

    public IEnumerable<(string, string)> GetNameIdPairs() => names.Zip(ids);
}
