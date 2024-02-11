using System.Globalization;
using CsvHelper.Configuration;

namespace PreonSharp.Loaders;

public class NcbiGeneTsvConfiguration
{
    public string? TsvFile { get; set; }

    public int NameColumn { get; set; } = 8;

    public int IdColumn { get; set; } = 2;

    public int SkipRows { get; set; } = 1;

    public CsvConfiguration CsvReaderConfiguration { get; } = new(CultureInfo.InvariantCulture)
    {
        Delimiter = "\t",
        Quote = '\0',
        Mode = CsvMode.NoEscape,
        WhiteSpaceChars = [' '],
    };
}
