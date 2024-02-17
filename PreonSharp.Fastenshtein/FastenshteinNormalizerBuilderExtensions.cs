namespace PreonSharp.Fastenshtein;

public static class FastenshteinNormalizerBuilderExtensions
{
    public static INormalizerBuilder AddFastenshteinMatchStrategy(this INormalizerBuilder builder)
    {
        builder.AddMatchStrategy<LevenshteinMatchStrategy>();
        return builder;
    }
}