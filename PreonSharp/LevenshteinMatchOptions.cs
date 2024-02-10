namespace PreonSharp;

public record class LevenshteinMatchOptions(
    decimal Threshold,
    int Decimals
)
{
    public LevenshteinMatchOptions() : this(0.2m, 3)
    {
    }
};
