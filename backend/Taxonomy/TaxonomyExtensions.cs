using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Normalizer;

namespace Taxonomy;

public static class TaxonomyExtensions
{
    public static IServiceCollection AddTaxonomy(this IServiceCollection serviceCollection,
        Action<ITaxonomyBuilder> configure)
    {
        serviceCollection
            .AddSingleton<IEntityProvider, EntityProvider>()
            .AddSingleton<IHostedService>(sp => sp.GetRequiredService<IEntityProvider>());
        serviceCollection
            .AddSingleton<EntitySearcher>()
            .AddSingleton<IHostedService>(sp => sp.GetRequiredService<EntitySearcher>());
        var builder = new TaxonomyBuilder(serviceCollection);
        configure(builder);
        return serviceCollection;
    }
    
    public static void AddTaxonomyKnowledge(this INormalizerBuilder builder)
    {
        builder.Services.AddSingleton<IKnowledgeProvider, TaxonomyKnowledgeProvider>();
    }
}