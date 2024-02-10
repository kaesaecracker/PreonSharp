using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp.Loaders;

internal sealed class NcbiTaxonomyProvider(
    [FromKeyedServices("NcbiTaxonomyTsv")] IReaderConfiguration readerConfig,
    ILogger<NcbiTaxonomyProvider> logger,
    string path) : IKnowledgeProvider
{
    public string SourceName => $"{nameof(NcbiTaxonomyProvider)} {path}";

    public IEnumerable<(string, string)> GetNameIdPairs()
    {
        logger.LogInformation("loading file {}", path);
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, readerConfig);

        var nameIndex = 1;
        var idIndex = 0;
        while (csv.Read())
        {
            var name = csv.GetField(nameIndex);
            if (string.IsNullOrWhiteSpace(name))
                continue;

            var id = csv.GetField(idIndex);
            yield return (name, id);
        }
    }
}
