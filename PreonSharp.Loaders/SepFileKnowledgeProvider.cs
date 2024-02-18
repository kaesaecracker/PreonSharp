using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace PreonSharp.Loaders;

public class SepFileKnowledgeProvider(SepFileSpec spec, ILogger<SepFileKnowledgeProvider> logger) : IKnowledgeProvider
{
    private readonly ILogger _logger = logger;

    public IEnumerable<(string, string)> GetNameIdPairs()
    {
        _logger.LogInformation("opening file {}", spec.FilePath);
        var csvReaderConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = spec.Separator.ToString(),
            HasHeaderRecord = spec.HasHeader,
            Mode = spec.Unquote ? CsvMode.RFC4180 : CsvMode.NoEscape,
        };

        var csvReader = new CsvReader(new StreamReader(spec.FilePath), csvReaderConfig);
        if (spec.HasHeader)
        {
            csvReader.Read();
            csvReader.ReadHeader();
        }

        return StreamRead(csvReader);
    }

    private IEnumerable<(string, string)> StreamRead(CsvReader csvReader)
    {
        while (csvReader.Read()){
            var name = csvReader[spec.NameColumnIndex];
            if (string.IsNullOrWhiteSpace(name))
                continue; // no name

            name = name.Trim();
            var id = csvReader[spec.IdColumnIndex].Trim();
            yield return (name, id);
        }

        csvReader.Dispose();
    }
}
