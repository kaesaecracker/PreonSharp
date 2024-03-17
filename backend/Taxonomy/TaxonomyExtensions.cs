using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Normalizer;

namespace Taxonomy;

public static class TaxonomyExtensions
{
    public static IServiceCollection AddTaxonomy(this IServiceCollection serviceCollection,
        Action<ITaxonomyBuilder> configure)
    {
        serviceCollection.AddSingleton<IEntityProvider, EntityProvider>();
        serviceCollection.AddSingleton<IHostedService, IEntityProvider>(sp => sp.GetRequiredService<IEntityProvider>());
        var builder = new TaxonomyBuilder(serviceCollection);
        configure(builder);
        return serviceCollection;
    }
    
    public static void AddTaxonomyKnowledge(this INormalizerBuilder builder)
    {
        builder.Services.AddSingleton<IKnowledgeProvider, TaxonomyKnowledgeProvider>();
    }
}