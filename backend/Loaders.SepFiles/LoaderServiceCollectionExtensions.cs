using Microsoft.Extensions.DependencyInjection;
using Normalizer;

namespace Loaders.SepFiles;

public static class LoaderServiceCollectionExtensions
{
    public static void AddSepFiles(this INormalizerBuilder builder)
    {
        builder.Services.AddSingleton<IKnowledgeProviderFactory, SepFilesKnowledgeProviderFactory>();
    }
}