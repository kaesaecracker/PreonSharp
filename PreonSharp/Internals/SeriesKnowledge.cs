namespace PreonSharp.Internals;

internal sealed class SeriesKnowledge(string kbName, IAsyncEnumerable<string> names, IAsyncEnumerable<string> ids) : IKnowledgeProvider
{
    public string SourceName => kbName;

    public IAsyncEnumerable<(string, string)> GetNameIdPairs() => names.Zip(ids);
}
