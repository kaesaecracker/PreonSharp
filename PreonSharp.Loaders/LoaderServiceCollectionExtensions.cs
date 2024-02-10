using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PreonSharp.Loaders;

public static class LoaderServiceCollectionExtensions
{
    public static INormalizerBuilder AddEbiTsv(this INormalizerBuilder builder, string path)
    {
        builder.Services.AddSingleton<IKnowledgeProvider>(sp =>
            ActivatorUtilities.CreateInstance<EbiTsvKnowledgeProvider>(sp, path));
        builder.Services.TryAddKeyedSingleton<IReaderConfiguration>("EbiTsv",
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
            });
        return builder;
    }

    public static INormalizerBuilder AddNcbiGeneTsv(this INormalizerBuilder builder, string path)
    {
        builder.Services.AddSingleton<IKnowledgeProvider>(sp =>
            ActivatorUtilities.CreateInstance<NcbiGeneTsvKnowledgeProvider>(sp, path));
        builder.Services.TryAddKeyedSingleton<IReaderConfiguration>("NcbiGeneTsv",
            (_, _) => new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                Quote = '\0',
                Mode = CsvMode.NoEscape,
            });
        return builder;
    }

    public static INormalizerBuilder AddNcbiTaxonomy(this INormalizerBuilder builder, string path)
    {
        builder.Services.AddSingleton<IKnowledgeProvider>(sp =>
            ActivatorUtilities.CreateInstance<NcbiTaxonomyProvider>(sp, path));
        builder.Services.TryAddKeyedSingleton<IReaderConfiguration>("NcbiTaxonomyTsv",
            (_, _) => new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                HasHeaderRecord = false,
                Quote = '\0',
                Mode = CsvMode.NoEscape,
            });
        return builder;
    }
}
