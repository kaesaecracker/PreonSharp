using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Taxonomy;
using Taxonomy.Models;

namespace Loaders.Ncbi;

// https://www.nlm.nih.gov/research/umls/sourcereleasedocs/current/NCBI/sourcerepresentation.html

internal sealed class NcbiNamesLoader(
    IOptions<NcbiConfiguration> config
) : IEntityLoader
{
    private readonly string _dataRoot = config.Value.TaxonomyDataRoot
                                        ?? throw new ConfigurationException("Ncbi DataRoot not specified");

    private readonly HashSet<string> _wantedNameClasses =
    [
        "synonym", "scientific name", "blast name", "genbank common name", "equivalent name",
        "common name", "acronym", "genbank acronym"
    ];

    public async Task Load(IEntityProviderBuilder builder)
    {
        using var csvReader = new CsvReader(
            new StreamReader($"{_dataRoot}/names.dmp"),
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                HasHeaderRecord = false,
                Mode = CsvMode.NoEscape
            });

        HashSet<EntityTag> names = [];
        HashSet<EntityTag> tags = [];
        ulong currentId = 1;

        while (await csvReader.ReadAsync())
        {
            var id = csvReader.GetField<ulong>(0);
            if (id != currentId)
            {
                await builder.AddEntity(NcbiIdNamespaces.TaxId, currentId.ToString(), names, tags);

                names = [];
                tags = [];
                currentId = id;
            }

            var tagText = csvReader[2];
            if (string.IsNullOrWhiteSpace(tagText))
                tagText = csvReader[1];
            tagText = tagText.Trim();

            var nameClass = csvReader[3].Trim();
            var entityTag = new EntityTag(nameClass, tagText);

            if (_wantedNameClasses.Contains(nameClass))
                names.Add(entityTag);
            else
                tags.Add(entityTag);
        }

        if (currentId != 1)
            await builder.AddEntity(NcbiIdNamespaces.TaxId, currentId.ToString(), names, tags);
    }
}