using System.Threading.Channels;

namespace Loaders.Ncbi;

public class NcbiTaxonomyKnowledgeProvider(NcbiTaxonomyProvider ncbiTaxonomyProvider) : IKnowledgeProvider
{
    public async Task WriteKnowledgeTo(ChannelWriter<KnowledgeDataPoint> outChannel)
    {
        await ncbiTaxonomyProvider.Started;

        foreach (var entity in ncbiTaxonomyProvider.All)
        foreach (var tag in entity.Tags)
        {
            await outChannel.WriteAsync(new KnowledgeDataPoint(
                $"/taxonomy/{entity.TaxonomyId}", tag.Value, "NCBI Taxonomy"));
        }
    }
}