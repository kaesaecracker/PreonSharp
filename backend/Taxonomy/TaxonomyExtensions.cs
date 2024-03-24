using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Normalizer;
using Normalizer.Internals;
using Taxonomy.Internals;

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
            .AddSingleton<IEntitySearcher, EntitySearcher>()
            .AddSingleton<IHostedService>(sp => sp.GetRequiredService<IEntitySearcher>());
        serviceCollection.AddSingleton<IUnifiedSearcher, UnifiedSearcher>();
        serviceCollection.TryAddSingleton<INameTransformer, DefaultNameTransformer>();
        var builder = new TaxonomyBuilder(serviceCollection);
        configure(builder);
        return serviceCollection;
    }

    public static void AddTaxonomyKnowledge(this INormalizerBuilder builder)
    {
        builder.Services.AddSingleton<IKnowledgeProvider, TaxonomyKnowledgeProvider>();
    }
}