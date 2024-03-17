using System.Threading.Channels;
using Normalizer;

namespace Taxonomy;

public class TaxonomyKnowledgeProvider(IEntityProvider entityProvider) : IKnowledgeProvider
{
    public async Task WriteKnowledgeTo(ChannelWriter<KnowledgeDataPoint> outChannel)
    {
        await entityProvider.Started;

        foreach (var entity in entityProvider.All)
        foreach (var tag in entity.Names)
        {
            await outChannel.WriteAsync(new KnowledgeDataPoint(
                entity.Id.ToString(), tag.Value, "Taxonomy"));
        }
    }
}