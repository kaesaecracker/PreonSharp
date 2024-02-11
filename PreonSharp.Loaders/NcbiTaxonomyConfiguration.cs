using System.Globalization;
using CsvHelper.Configuration;

namespace PreonSharp.Loaders;

public class NcbiTaxonomyConfiguration
{
    public CsvConfiguration CsvReaderConfiguration { get; } = new(CultureInfo.InvariantCulture)
    {
        Delimiter = "|",
        HasHeaderRecord = false,
        Quote = '\0',
        Mode = CsvMode.NoEscape,
        WhiteSpaceChars = [' ', '\t'],
    };

    public string? NamesDmpFile { get; set; }

    public int NamesDmpNameColumn { get; set; } = 1;
    public int NamesDmpIdColumn { get; set; } = 0;

}
