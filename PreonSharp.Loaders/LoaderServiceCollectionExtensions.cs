using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp.Loaders;

public static class LoaderServiceCollectionExtensions
{
    public static void AddSepFile(this INormalizerBuilder builder, SepFileSpec spec)
    {
        builder.Services.AddSingleton<IKnowledgeProvider>(sp =>
            ActivatorUtilities.CreateInstance<SepFileKnowledgeProvider>(sp, spec));
    }

    public static void AddSepFiles(this INormalizerBuilder builder)
    {
        builder.Services.AddSingleton<IKnowledgeProviderFactory, SepFilesKnowledgeProviderFactory>();
    }
}