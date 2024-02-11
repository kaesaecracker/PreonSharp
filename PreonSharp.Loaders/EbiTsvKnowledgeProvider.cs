using System.IO;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp.Loaders;

internal sealed class EbiTsvKnowledgeProvider(
    [FromKeyedServices("EbiTsv")] IReaderConfiguration ebiTsv,
    ILogger<EbiTsvKnowledgeProvider> logger,
    string path) : IKnowledgeProvider
{
    public string SourceName => $"{nameof(EbiTsvKnowledgeProvider)} {path}";

    public async IAsyncEnumerable<(string, string)> GetNameIdPairs()
    {
        logger.LogInformation("loading file {}", path);
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, ebiTsv);

        await csv.ReadAsync();
        csv.ReadHeader();
        var nameIndex = csv.GetFieldIndex("Name");
        var idIndex = csv.GetFieldIndex("ChEMBL ID");
        while (await csv.ReadAsync())
        {
            var name = csv.GetField(nameIndex);
            if (string.IsNullOrWhiteSpace(name))
                continue;

            var id = csv.GetField(idIndex);
            yield return (name, id);
        }
    }
}
