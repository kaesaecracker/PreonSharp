using System.Buffers;
using PreonSharp.Levenshtein;

namespace PreonBenchmarks;

public class LevenshteinDistanceBenchmarks
{
    private readonly ILevenshteinCosts _unweightedCosts = new UnweightedLevenshteinCosts();
    private readonly Levenshtein _lev = new(ArrayPool<int>.Shared, new UnweightedLevenshteinCosts());

    [Benchmark]
    public int CalculateDistanceOnFreshInstance()
    {
        var lev = new Levenshtein(ArrayPool<int>.Create(), _unweightedCosts);
        return lev.CalculateDistance("kiten", "sitteng");
    }

    [Benchmark]
    public int CalculateDistanceOnFreshSharedPoolInstance()
    {
        var lev = new Levenshtein(ArrayPool<int>.Shared, _unweightedCosts);
        return lev.CalculateDistance("kiten", "sitteng");
    }
    
    [Benchmark]
    public int CalculateDistanceOnExistingPooledInstance()
    {
        return _lev.CalculateDistance("kiten", "sitteng");
    }
    
    [Benchmark]
    public int CalculateDistanceOnNoopPoolInstance()
    { var lev = new Levenshtein(new NoopArrayPool<int>(), _unweightedCosts);
        return lev.CalculateDistance("kiten", "sitteng");
    }
}