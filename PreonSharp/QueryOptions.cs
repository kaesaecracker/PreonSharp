namespace PreonSharp;

public record class QueryOptions(
    MatchType MatchType = MatchType.All,
    decimal Threshold = 0.2m,
    int NGrams = 1,
    int Decimals = 3
);
