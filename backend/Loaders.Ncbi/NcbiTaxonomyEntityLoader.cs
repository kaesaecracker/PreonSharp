using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Taxonomy;
using Taxonomy.Models;

namespace Loaders.Ncbi;

// https://www.nlm.nih.gov/research/umls/sourcereleasedocs/current/NCBI/sourcerepresentation.html

internal sealed class NcbiTaxonomyEntityLoader(
    ILogger<NcbiTaxonomyEntityLoader> logger,
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
        logger.LogInformation("starting taxonomy provider");
        var hierarchy = await LoadNodes($"{_dataRoot}/nodes.dmp");
        logger.LogDebug("loaded hierarchy");
        var entities = await LoadEntities($"{_dataRoot}/names.dmp", hierarchy);
        logger.LogInformation("loaded {} entities", entities.Count);


        var source = builder.AddIdNamespace(NcbiIdNamespaces.TaxId);
        var idMap = new Dictionary<ulong, Guid>();
        var parentMap = new Dictionary<Guid, ulong>();
        foreach (var e in entities.Values)
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

    private static async Task<Dictionary<ulong, ulong>> LoadNodes(string dmpFile)
    {
        using var csvReader = new CsvReader(
            new StreamReader(dmpFile),
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                HasHeaderRecord = false,
                Mode = CsvMode.NoEscape
            });

        var result = new Dictionary<ulong, ulong>();
        while (await csvReader.ReadAsync())
        {
            var id = csvReader.GetField<ulong>(0);
            var parent = csvReader.GetField<ulong>(1);
            if (id == parent)
                continue;

            result[id] = parent;
        }

        return result;
    }

    private async Task<Dictionary<ulong, NcbiTaxonomyEntity>> LoadEntities(string dmpFile,
        Dictionary<ulong, ulong> hierarchy)
    {
        using var csvReader = new CsvReader(
            new StreamReader(dmpFile),
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                HasHeaderRecord = false,
                Mode = CsvMode.NoEscape
            });

        Dictionary<ulong, NcbiTaxonomyEntity> entities = [];
        HashSet<EntityTag> names = [];
        HashSet<EntityTag> tags = [];
        ulong currentId = 1;
        while (await csvReader.ReadAsync())
        {
            var id = csvReader.GetField<ulong>(0);
            if (id != currentId)
            {
                ulong? parent;
                if (hierarchy.TryGetValue(currentId, out var foundParent))
                    parent = foundParent;
                else
                    parent = null;

                var entity = new NcbiTaxonomyEntity(currentId, names, tags, parent);
                entities.Add(id, entity);

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

        return entities;
    }
}