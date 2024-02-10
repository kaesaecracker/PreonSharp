using Microsoft.Extensions.DependencyInjection.Extensions;
using PreonSharp.Internals;

namespace PreonSharp;

public static class NormalizerServiceCollectionExtensions
{
    public static IServiceCollection AddNormalizer(this IServiceCollection services, Action<INormalizerBuilder> configure)
    {
        configure(new NormalizerBuilder(services));
        services.TryAddSingleton<INormalizer, Normalizer>();
        services.TryAddSingleton<INameTransformer, DefaultNameTransformer>();
        services.TryAddSingleton<KnowledgeAggregator>();
        return services;
    }
}
