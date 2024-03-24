using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Taxonomy;
using Taxonomy.Models;

namespace Loaders.Ncbi;

public class NcbiImageLoader : IEntityLoader
{
    private readonly string _filePath;

    public NcbiImageLoader(IOptions<NcbiConfiguration> config)
    {
        var dataRoot = config.Value.TaxonomyDataRoot
                       ?? throw new ConfigurationException("Ncbi DataRoot not specified");
        _filePath = Path.Join(dataRoot, "images.dmp");
    }

    public async Task Load(IEntityProviderBuilder builder)
    {
        using var csvReader = new CsvReader(
            new StreamReader(_filePath),
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                HasHeaderRecord = true,
                Mode = CsvMode.NoEscape
            });

        while (await csvReader.ReadAsync())
        {
            var imageId = csvReader[0].Trim();
            var names = new HashSet<EntityTag>([
                new EntityTag("name", csvReader[1].Trim())
            ]);
            var tags = new HashSet<EntityTag>([
                new EntityTag("url", csvReader[2].Trim()),
                new EntityTag("license", csvReader[3].Trim()),
                new EntityTag("attribution", csvReader[4].Trim()),
                new EntityTag("source", csvReader[5].Trim())
            ]);

            var image = await builder.AddEntity(NcbiIdNamespaces.Images, imageId, names, tags);

            var taxIds = csvReader[7].Trim()
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var taxId in taxIds)
            {
                var entity = await builder.ReferenceEntity(NcbiIdNamespaces.TaxId, taxId);
                await builder.AddRelation("has image", "image of", image, entity);
            }
        }
    }
}