using System.Diagnostics.CodeAnalysis;

namespace PreonSharp;

public static class NormalizerBuilderExtensions
{
    public static void AddSeries(this INormalizerBuilder builder, IEnumerable<string> names, IEnumerable<string> ids)
    {
        builder.Services.AddSingleton<IKnowledgeProvider>(new SeriesKnowledge(names, ids));
    }

    public static void AddMatchStrategy
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>
        (this INormalizerBuilder builder)
        where T : class, IMatchStrategy
    {
        builder.Services.AddSingleton<IMatchStrategy, T>();
    }
}