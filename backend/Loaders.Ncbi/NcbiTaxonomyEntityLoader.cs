using Microsoft.Extensions.Logging;
using Taxonomy;
using Taxonomy.Models;

namespace Loaders.Ncbi;

internal sealed class NcbiTaxonomyEntityLoader(
    NcbiTaxonomyProvider taxonomyProvider,
    ILogger<NcbiTaxonomyEntityLoader> logger)
    : IEntityLoader
{
    public async Task Load(IEntityProviderBuilder builder)
    {
        await taxonomyProvider.Started;
        var source = builder.AddIdNamespace("NCBI Taxonomy");
        var idMap = new Dictionary<ulong, Guid>();
        var parentMap = new Dictionary<Guid, ulong>();
        foreach (var e in taxonomyProvider.All)
        {
            var guid = builder.AddEntity(source, e.TaxonomyId.ToString(), e.Names, e.Tags);
            idMap.Add(e.TaxonomyId, guid);

            if (e.Parent == null)
                continue;

            if (idMap.TryGetValue(e.Parent.Value, out var parentGuid))
                builder.AddRelation("child", "parent", guid, parentGuid);
            else
                parentMap.Add(guid, e.Parent.Value);
        }

        foreach (var pair in parentMap)
        {
            if (!idMap.TryGetValue(pair.Value, out var value))
            {
                logger.LogWarning("parent {} not found", pair.Value);
                continue;
            }

            builder.AddRelation("child", "parent", pair.Key, value);
        }
    }
}