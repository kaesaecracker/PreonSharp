using System.Threading.Channels;

namespace PreonSharp.Internals;

internal sealed class SeriesKnowledge(IEnumerable<string> names, IEnumerable<string> ids) : IKnowledgeProvider
{
    public async Task WriteKnowledgeTo(ChannelWriter<KnowledgeDataPoint> outChannel)
    {
        foreach (var (name, id) in names.Zip(ids))
        {
            await outChannel.WriteAsync(new KnowledgeDataPoint(id, name, nameof(SeriesKnowledge)));
        }
    }
}
