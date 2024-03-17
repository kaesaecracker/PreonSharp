using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Normalizer;

public static class NormalizerServiceCollectionExtensions
{
    public static IServiceCollection AddNormalizer(this IServiceCollection services,
        Action<INormalizerBuilder> configure)
    {
        var builder = new NormalizerBuilder(services);
        configure(builder);
        builder.AddMatchStrategy<ExactMatchStrategy>();

        services.AddSingleton<Internals.Normalizer>();
        services.AddSingleton<KnowledgeAggregator>();

        services.AddSingleton<INormalizer, Internals.Normalizer>();
        services.AddSingleton<IHostedService, INormalizer>(sp => sp.GetRequiredService<INormalizer>());

        services.TryAddSingleton<INameTransformer, DefaultNameTransformer>();
        return services;
    }
}