using System.Threading.Channels;

namespace Normalizer;

public interface IKnowledgeProvider
{
    public Task WriteKnowledgeTo(ChannelWriter<KnowledgeDataPoint> outChannel);
}
