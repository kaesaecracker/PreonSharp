namespace PreonSharp;

public class LevenshteinMatchOptions
{
    public decimal Threshold { get; init; } = 0.2m;
    public int Decimals { get; init; } = 3;

    public ParallelOptions ParallelOptions { get; set; } = new();
};
