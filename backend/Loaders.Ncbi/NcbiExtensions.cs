using Microsoft.Extensions.DependencyInjection;
using Taxonomy;

namespace Loaders.Ncbi;

public static class NcbiExtensions
{
    public static IServiceCollection AddNcbiTaxonomy(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<NcbiTaxonomyProvider>();
        serviceCollection.AddHostedService(sp => sp.GetRequiredService<NcbiTaxonomyProvider>());
        return serviceCollection;
    } 
    
    public static void AddNcbiTaxonomyKnowledge(this INormalizerBuilder builder)
    {
        builder.Services.AddSingleton<IKnowledgeProvider, NcbiTaxonomyKnowledgeProvider>();
    }

    public static void AddNcbiEntityLoader(this ITaxonomyBuilder builder)
    {
        builder.Services.AddSingleton<IEntityLoader, NcbiTaxonomyEntityLoader>();
    }
}