using PreonSharp.Internals;

namespace PreonSharp;

public static class NormalizerBuilderExtensions
{
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
}
