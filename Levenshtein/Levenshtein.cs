using System.Buffers;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Levenshtein;

public sealed class Levenshtein(ArrayPool<int> pool, ILevenshteinCosts costFunctions)
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public int CalculateDistance(string s1, string s2)
    {
        // based on https://en.wikipedia.org/wiki/Levenshtein_distance iterative with two matrix rows

        var s1Length = s1.Length;
        var s2Length = s2.Length;

        if (string.IsNullOrEmpty(s1))
            return s2.Sum(costFunctions.GetInsertCost);

        if (string.IsNullOrEmpty(s2))
            return s1.Sum(costFunctions.GetDeleteCost);

        var previousRow = pool.Rent(s1Length + 1);
        var currentRow = pool.Rent(s1Length + 1);

        for (var i = 0; i <= s1Length; i++)
            previousRow[i] = i;

        for (var j = 1; j <= s2Length; j++)
        {
            var currentChar = s2[j - 1];
            currentRow[0] = j;

            for (var i = 1; i <= s1Length; i++)
            {
                var deletionCosts = currentRow[i - 1] + costFunctions.GetDeleteCost(currentChar);
                var insertCosts = previousRow[i] + costFunctions.GetInsertCost(currentChar);
                var substitutionCost = previousRow[i - 1];
                if (s1[i - 1] != currentChar)
                    substitutionCost += costFunctions.GetSubstitutionCost(currentChar, s1[i - 1]);

                // minimum of cell to the left+1, to the top+1, diagonally left and up +cost				
                currentRow[i] = MathExtensions.Min(substitutionCost, deletionCosts, insertCosts);
            }

            // copy current distance counts to 'previous row' distance counts
            (previousRow, currentRow) = (currentRow, previousRow);
        }

        pool.Return(previousRow);
        pool.Return(currentRow);

        // our last action in the above loop was to switch d and p, so p now 
        // actually has the most recent cost counts
        return previousRow[s1Length];
    }
}