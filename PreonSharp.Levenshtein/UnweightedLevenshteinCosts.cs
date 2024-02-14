namespace PreonSharp.Levenshtein;

public sealed class UnweightedLevenshteinCosts : ILevenshteinCosts
{
    public int GetInsertCost(char charToInsert) => 1;
    public int GetDeleteCost(char charToDelete) => 1;
    public int GetSubstitutionCost(char fromChar, char toChar) => 1;
}