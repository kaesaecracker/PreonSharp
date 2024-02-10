using PreonSharp.Internals;

namespace PreonSharp;

public static class NormalizerBuilderExtensions
{
    [Obsolete("very inefficient")]
    public static INormalizerBuilder AddName(this INormalizerBuilder builder, string name, string id)
    {
        builder.Services.AddSingleton<IKnowledgeProvider>(new SinglePairKnowledge(name, id));
        return builder;
    }

    public static INormalizerBuilder AddSeries(this INormalizerBuilder builder, string kbName,
        IEnumerable<string> names,
        IEnumerable<string> ids)
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
