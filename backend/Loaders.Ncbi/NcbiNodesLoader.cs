using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Taxonomy;

namespace Loaders.Ncbi;

public sealed class NcbiNodesLoader(
    ILogger<NcbiNodesLoader> logger,
    IOptions<NcbiConfiguration> config
) : IEntityLoader
{
    private readonly string _dataRoot = config.Value.TaxonomyDataRoot
                                        ?? throw new ConfigurationException("Ncbi DataRoot not specified");

    public async Task Load(IEntityProviderBuilder builder)
    {
        using var csvReader = new CsvReader(
            new StreamReader($"{_dataRoot}/nodes.dmp"),
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                HasHeaderRecord = false,
                Mode = CsvMode.NoEscape
            });

        while (await csvReader.ReadAsync())
        {
            var childTaxId = csvReader[0].Trim();
            var childGuid = await builder.ReferenceEntity(NcbiIdNamespaces.TaxId, childTaxId);

            var parentTaxId = csvReader[1].Trim();
            if (childTaxId == parentTaxId)
                continue;

            var parentGuid = await builder.ReferenceEntity(NcbiIdNamespaces.TaxId, parentTaxId);
            await builder.AddRelation("child", "parent", childGuid, parentGuid);
        }

        logger.LogDebug("loaded hierarchy");
    }
}