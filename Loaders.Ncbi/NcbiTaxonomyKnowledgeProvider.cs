using System.Threading;
using System.Threading.Channels;

namespace Loaders.Ncbi;

public class NcbiTaxonomyKnowledgeProvider(TaxonomyProvider taxonomyProvider) : IKnowledgeProvider
{
    public async Task WriteKnowledgeTo(ChannelWriter<KnowledgeDataPoint> outChannel)
    {
        await taxonomyProvider.StartedAsync(CancellationToken.None);

        foreach (var entity in taxonomyProvider.All)
        foreach (var tag in entity.Tags)
        {
            await outChannel.WriteAsync(new KnowledgeDataPoint(
                $"/taxonomy/{entity.TaxonomyId}", tag.Value, "NCBI Taxonomy"));
        }
    }
}