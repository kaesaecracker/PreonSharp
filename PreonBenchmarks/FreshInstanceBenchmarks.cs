using System.Buffers;

namespace PreonBenchmarks;

public class FreshInstanceBenchmarks
{
    private readonly ILevenshteinCosts _unweightedCosts = new UnweightedLevenshteinCosts();
    private readonly PreonSharp.Levenshtein.Levenshtein _lev = new(ArrayPool<int>.Shared, new UnweightedLevenshteinCosts());

    [Benchmark]
    public int CalculateDistanceOnFreshInstance()
    {
        var lev = new PreonSharp.Levenshtein.Levenshtein(ArrayPool<int>.Create(), _unweightedCosts);
        return lev.CalculateDistance("kiten", "sitteng");
    }

    [Benchmark]
    public int CalculateDistanceOnFreshSharedPoolInstance()
    {
        var lev = new PreonSharp.Levenshtein.Levenshtein(ArrayPool<int>.Shared, _unweightedCosts);
        return lev.CalculateDistance("kiten", "sitteng");
    }
    
    [Benchmark]
    public int CalculateDistanceOnExistingPooledInstance()
    {
        return _lev.CalculateDistance("kiten", "sitteng");
    }
}