using Microsoft.Extensions.DependencyInjection.Extensions;
using PreonSharp;

namespace Levenshtein;

public static class LevenshteinServiceCollectionExtension
{
    public static void AddLevenshteinMatchStrategy(this INormalizerBuilder builder)
    {
        builder.AddMatchStrategy<MyLevenshteinMatchStrategy>();
        builder.Services.TryAddSingleton<ILevenshteinCosts, UnweightedLevenshteinCosts>();
    }
}