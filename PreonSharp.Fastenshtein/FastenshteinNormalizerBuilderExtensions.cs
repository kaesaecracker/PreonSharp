using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp.Fastenshtein;

public static class FastenshteinNormalizerBuilderExtensions
{
    public static INormalizerBuilder AddFastenshteinMatchStrategy(this INormalizerBuilder builder,
        Action<LevenshteinMatchOptions>? configure = null)
    {
        builder.AddMatchStrategy<LevenshteinMatchStrategy>();
        if (configure != null)
            builder.Services.Configure(configure);
        return builder;
    }
}