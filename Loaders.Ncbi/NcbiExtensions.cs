using Microsoft.Extensions.DependencyInjection;

namespace Loaders.Ncbi;

public static class NcbiExtensions
{
    public static IServiceCollection AddNcbiTaxonomy(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<TaxonomyProvider>();
        serviceCollection.AddHostedService(sp => sp.GetRequiredService<TaxonomyProvider>());
        return serviceCollection;
    } 
    
    public static void AddNcbiTaxonomyKnowledge(this INormalizerBuilder builder)
    {
        builder.Services.AddSingleton<IKnowledgeProvider, NcbiTaxonomyKnowledgeProvider>();
    }
}