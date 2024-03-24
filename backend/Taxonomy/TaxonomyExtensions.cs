using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Taxonomy.Internals;
using DefaultNameTransformer = Taxonomy.Internals.DefaultNameTransformer;

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
        var builder = new TaxonomyBuilder(serviceCollection);
        configure(builder);
        serviceCollection.TryAddSingleton<INameTransformer, DefaultNameTransformer>();
        serviceCollection.TryAddSingleton<IUnifiedSearcher, UnifiedSearcher>();
        return serviceCollection;
    }
}