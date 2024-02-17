namespace PreonSharp.Levenshtein;

public static class LevenshteinServiceCollectionExtension
{
    public static void AddLevenshteinMatchStrategy(this INormalizerBuilder builder)
    {
        builder.AddMatchStrategy<MyLevenshteinMatchStrategy>();
    }
}