using System.Globalization;
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

    public static INormalizerBuilder AddNcbiGeneTsv(this INormalizerBuilder builder,
        Action<NcbiGeneTsvConfiguration> configure)
    {
        builder.Services.AddSingleton<IKnowledgeProvider, NcbiGeneTsvKnowledgeProvider>();
        builder.Services.Configure(configure);
        return builder;
    }

    public static INormalizerBuilder AddNcbiTaxonomy(this INormalizerBuilder builder,
        Action<NcbiTaxonomyConfiguration> configure)
    {
        builder.Services.AddSingleton<IKnowledgeProvider, NcbiTaxonomyProvider>();
        builder.Services.Configure(configure);
        return builder;
    }
}
