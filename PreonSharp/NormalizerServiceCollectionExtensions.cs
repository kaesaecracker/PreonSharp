using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PreonSharp;

public static class NormalizerServiceCollectionExtensions
{
    public static IServiceCollection AddNormalizer(this IServiceCollection services, Action<INormalizerBuilder> configure)
    {
        var builder = new NormalizerBuilder(services);
        configure(builder);
        builder.AddMatchStrategy<ExactMatchStrategy>();
        
        services.TryAddSingleton<INormalizer, Normalizer>();
        services.TryAddSingleton<INameTransformer, DefaultNameTransformer>();
        services.TryAddSingleton<KnowledgeAggregator>();
        return services;
    }
}
