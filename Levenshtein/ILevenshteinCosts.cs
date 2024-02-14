namespace Levenshtein;

public interface ILevenshteinCosts
{
    int GetInsertCost(char charToInsert);
    int GetDeleteCost(char charToDelete);
    int GetSubstitutionCost(char fromChar, char toChar);
}