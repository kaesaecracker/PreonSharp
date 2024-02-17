using BenchmarkDotNet.Engines;
using Fastenshtein;

namespace PreonBenchmarks;

public class FastenshteinDistanceBenchmarks
{
    [Params(1, 10, 100)] 
    public int Repeats { get; set; }

    private readonly Consumer _consumer = new();

    [Benchmark]
    public void CalculateDistanceWithStaticMethod()
    {
        for (int i = 0; i < Repeats; i++)
        {
            _consumer.Consume(Levenshtein.Distance("kiten", "sitteng"));
        }
    }
}