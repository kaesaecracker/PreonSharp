using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace PreonSharp.Loaders;

public static class EbiLoader
{
    public static PrecisionOncologyNormalizerBuilder LoadEbi(
        this PrecisionOncologyNormalizerBuilder builder,
        string path)
    {
        var csvReaderConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t",
        };

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, csvReaderConfig);

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
            builder.Fit(name, id);
        }

        return builder;
    }
}
