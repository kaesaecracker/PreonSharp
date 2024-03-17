using Fastenshtein;
using Normalizer.Levenshtein;

namespace Benchmarks;

public class SortByLevenshteinBenchmark
{
    private string[] Words =>
    [
        "sitteng",
        "kiten",
        "apple",
        "banana",
        "airplane",
        "supercalifragilisticexpialidocious",
        "all your benchmark are belong to us",
        "i \u2665\ufe0f unicode",
        "",
        "test",
        "rindfleischetikettierungsüberwachungsaufgabenübertragungsgesetz",
    ];

    private const string SearchTerm = "banana plane";

    [Benchmark(Baseline = true)]
    public string[] WithoutSorting()
    {
        var array = Words;
        Array.Sort(array, (a, b) => a.Length - b.Length);
        return array;
    }


    private readonly LevenshteinSearch _ownLev = new(SearchTerm);

    [Benchmark]
    public string[] WithOwnLevenshtein()
    {
        var array = Words;
        Array.Sort(array, (a, b) => _ownLev.DistanceFrom(a) - _ownLev.DistanceFrom(b));
        return array;
    }

    private readonly Levenshtein _fastLev = new(SearchTerm);

    [Benchmark]
    public string[] WithFastenshtein()
    {
        var array = Words;
        Array.Sort(array, (a, b) => _fastLev.DistanceFrom(a) - _fastLev.DistanceFrom(b));
        return array;
    }
}