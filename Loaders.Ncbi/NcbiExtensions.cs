using Microsoft.Extensions.DependencyInjection;

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
}