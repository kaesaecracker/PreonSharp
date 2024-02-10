using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace PreonSharp.Loaders;

internal class EbiCsvKnowledgeProvider(string path) : IKnowledgeProvider
{
    private readonly CsvConfiguration _csvReaderConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = "\t",
    };

    public IEnumerable<(string, string)> GetNameIdPairs()
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, _csvReaderConfig);

        int i = 0;
        csv.Read();
        csv.ReadHeader();
        while (csv.Read())
        {
            i++;
            var name = csv.GetField<string>("Name");
            if (string.IsNullOrWhiteSpace(name))
                continue;

            var id = csv.GetField<string>("ChEMBL ID");
            yield return (name, id);
        }
    }
}
