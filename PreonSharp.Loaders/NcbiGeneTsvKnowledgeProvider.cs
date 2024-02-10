using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp.Loaders;

internal sealed class NcbiGeneTsvKnowledgeProvider(
    [FromKeyedServices("NcbiTsv")] IReaderConfiguration ebiTsv,
    ILogger<NcbiGeneTsvKnowledgeProvider> logger,
    string path) : IKnowledgeProvider
{
    public string SourceName => $"{nameof(NcbiGeneTsvKnowledgeProvider)} {path}";

    public IEnumerable<(string, string)> GetNameIdPairs()
    {
        logger.LogInformation("loading file {}", path);
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, ebiTsv);

        csv.Read();
        csv.ReadHeader();
        var nameIndex = csv.GetFieldIndex("description");
        var idIndex = csv.GetFieldIndex("Symbol");
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
