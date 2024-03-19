using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using Taxonomy;
using Taxonomy.Models;

namespace Loaders.Ncbi;

public class NcbiGeneInfoEntityLoader(IOptions<NcbiConfiguration> configuration) : IEntityLoader
{
    private readonly string _filePath = configuration.Value.GeneInfo.File
                                        ?? throw new ArgumentException("GeneInfoFile not set", nameof(configuration));

    private readonly bool _loadExtendedTags = configuration.Value.GeneInfo.LoadExtendedTags;
    private readonly uint? _sample = configuration.Value.GeneInfo.Sample;

    public async Task Load(IEntityProviderBuilder builder)
    {
        using var csvReader = new CsvReader(
            new StreamReader(_filePath),
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                HasHeaderRecord = true,
                Mode = CsvMode.NoEscape
            });

        var taxIdSource = builder.AddIdNamespace(NcbiIdNamespaces.TaxId);
        var geneIdNamespace = builder.AddIdNamespace(NcbiIdNamespaces.GeneInfo);
        var currentTaxId = string.Empty;
        var currentTaxGuid = Guid.Empty;

        ulong rowId = 0;
        while (await csvReader.ReadAsync())
        {
            rowId++;
            if (_sample != null && rowId % _sample != 0)
                continue;

            var taxId = csvReader[0].Trim();
            if (taxId != currentTaxId)
            {
                currentTaxGuid = builder.ReferenceEntity(taxIdSource, taxId);
                currentTaxId = taxId;
            }

            var geneId = csvReader[1].Trim();
            HashSet<EntityTag> names = [new EntityTag("gene symbol", csvReader[2].Trim())];
            HashSet<EntityTag> tags = [new EntityTag("type of gene", csvReader[8].Trim())];

            if (_loadExtendedTags)
            {
                names.Add(new EntityTag("LOCUS tag", csvReader[3].Trim()));
                names.Add(new EntityTag("synonyms", csvReader[4].Trim()));
                names.Add(new EntityTag("Symbol_from_nomenclature_authority", csvReader[9].Trim()));
                names.Add(new EntityTag("Full_name_from_nomenclature_authority", csvReader[10].Trim()));

                tags.Add(new EntityTag("description", csvReader[7].Trim()));
            }

            var geneEntity = builder.AddEntity(geneIdNamespace, geneId, names, tags);
            builder.AddRelation("has gene", "gene of", geneEntity, currentTaxGuid);
        }
    }
}