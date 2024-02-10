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

    public static INormalizerBuilder AddSeries(this INormalizerBuilder builder, IEnumerable<string> names,
        IEnumerable<string> ids)
    {
        builder.Services.AddSingleton<IKnowledgeProvider>(new SeriesKnowledge(names, ids));
        return builder;
    }
}
