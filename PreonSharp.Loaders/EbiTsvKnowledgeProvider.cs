using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp.Loaders;

internal class EbiTsvKnowledgeProvider(
    [FromKeyedServices("EbiTsv")] IReaderConfiguration ebiTsv,
    ILogger<EbiTsvKnowledgeProvider> logger,
    string path) : IKnowledgeProvider
{
    public IEnumerable<(string, string)> GetNameIdPairs()
    {
        logger.LogInformation("loading file {}", path);
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, ebiTsv);

        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            var name = csv.GetField<string>("Name");
            if (string.IsNullOrWhiteSpace(name))
                continue;

            var id = csv.GetField<string>("ChEMBL ID");
            yield return (name, id);
        }
    }
}
