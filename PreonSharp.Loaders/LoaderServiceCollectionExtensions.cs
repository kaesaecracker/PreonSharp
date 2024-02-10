using System.Globalization;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp.Loaders;

public static class LoaderServiceCollectionExtensions
{
    public static INormalizerBuilder AddEbiTsv(this INormalizerBuilder builder, string path)
    {
        builder.Services.AddSingleton<IKnowledgeProvider>(sp =>
            ActivatorUtilities.CreateInstance<EbiTsvKnowledgeProvider>(sp, path));
        builder.Services.AddKeyedSingleton<IReaderConfiguration>("EbiTsv",
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
            });
        return builder;
    }
}
