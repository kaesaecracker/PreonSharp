using PreonSharp.Internals;

namespace PreonSharp;

public static class NormalizerBuilderExtensions
{
    public static INormalizerBuilder AddSeries(this INormalizerBuilder builder, string kbName,
        IAsyncEnumerable<string> names,
        IAsyncEnumerable<string> ids)
    {
        builder.Services.AddSingleton<IKnowledgeProvider>(new SeriesKnowledge(kbName, names, ids));
        return builder;
    }

    public static INormalizerBuilder AddMatchStrategy<T>(this INormalizerBuilder builder)
        where T : class, IMatchStrategy
    {
        builder.Services.AddSingleton<IMatchStrategy, T>();
        return builder;
    }

    public static INormalizerBuilder AddExactMatchStrategy(this INormalizerBuilder builder)
    {
        builder.AddMatchStrategy<ExactMatchStrategy>();
        return builder;
    }

    public static INormalizerBuilder AddLevenshteinMatchStrategy(this INormalizerBuilder builder,
        Action<LevenshteinMatchOptions>? configure = null)
    {
        builder.AddMatchStrategy<LevenshteinMatchStrategy>();
        if (configure != null)
            builder.Services.Configure<LevenshteinMatchOptions>(configure);
        return builder;
    }
}
