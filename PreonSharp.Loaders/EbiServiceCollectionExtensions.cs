using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp.Loaders;

public static class EbiServiceCollectionExtensions
{
    public static INormalizerBuilder AddEbiCsv(this INormalizerBuilder builder, string path)
    {
        builder.Services.AddSingleton<IKnowledgeProvider>(sp =>
            ActivatorUtilities.CreateInstance<EbiCsvKnowledgeProvider>(sp, path));
        return builder;
    }
}
