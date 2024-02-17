using System;

namespace PreonSharp.Levenshtein;

public class LevenshteinSearch
{
    private readonly string _searchTerm;
    private readonly int[] _a;
    private readonly int[] _b;

    public LevenshteinSearch(string searchTerm)
    {
        _searchTerm = searchTerm;

        var len = _searchTerm.Length + 1;
        _a = new int[len];
        _b = new int[len];
    }

    public int DistanceFrom(string other)
    {
        // based on https://en.wikipedia.org/wiki/Levenshtein_distance iterative with two matrix rows

        var otherLength = other.Length;
        if (_searchTerm == string.Empty)
            return otherLength;
        var searchTermLength = _searchTerm.Length;
        if (other == string.Empty)
            return searchTermLength;

        var previousRow = _a;
        var currentRow = _b;

        for (var i = 0; i <= searchTermLength; i++)
            previousRow[i] = i;

        for (var j = 1; j <= otherLength; j++)
        {
            var currentChar = other[j - 1];
            currentRow[0] = j;

            for (var i = 1; i <= searchTermLength; i++)
            {
                var prevI = i - 1;
                var deletionCosts = currentRow[prevI] + 1;
                var insertCosts = previousRow[i] + 1;
                var substitutionCost = previousRow[prevI] + (_searchTerm[prevI] == currentChar ? 0 : 1);

                currentRow[i] = Math.Min(substitutionCost, Math.Min(deletionCosts, insertCosts));
            }

            (previousRow, currentRow) = (currentRow, previousRow);
        }

        // our last action in the above loop was to switch d and p, so p now 
        // actually has the most recent cost counts
        return previousRow[searchTermLength];
    }
}