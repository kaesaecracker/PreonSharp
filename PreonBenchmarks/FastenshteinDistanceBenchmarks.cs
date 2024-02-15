using Fastenshtein;

namespace PreonBenchmarks;

public class FastenshteinDistanceBenchmarks
{
    [Benchmark]
    public int CalculateDistanceWithStaticMethod()
    {
        return Levenshtein.Distance("kiten", "sitteng");
    }

}