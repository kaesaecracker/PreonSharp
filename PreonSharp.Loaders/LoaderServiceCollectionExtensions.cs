using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp.Loaders;

public static class LoaderServiceCollectionExtensions
{
    public static INormalizerBuilder AddSepFile(this INormalizerBuilder builder, SepFileSpec spec)
    {
        builder.Services.AddSingleton<IKnowledgeProvider>(sp => ActivatorUtilities.CreateInstance<SepFileKnowledgeProvider>(sp, spec));
        return builder;
    }
}
