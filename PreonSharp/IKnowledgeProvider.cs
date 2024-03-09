using System.Threading.Channels;

namespace PreonSharp;

public interface IKnowledgeProvider
{
    public Task WriteKnowledgeTo(ChannelWriter<KnowledgeDataPoint> outChannel);
}
