using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Levenshtein;

internal static class Levenshtein
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static int CalculateDistance(string s1, string s2)
    {
        // based on http://web.archive.org/web/20120526085419/http://www.merriampark.com/ldjava.htm

        var s1Length = s1.Length;
        var s2Length = s2.Length;

        if (string.IsNullOrEmpty(s1))
            return s2.Length;

        if (string.IsNullOrWhiteSpace(s2))
            return s1.Length;

        var p = ArrayPool<int>.Shared.Rent(s1Length + 1); //'previous' cost array, horizontally
        var d = ArrayPool<int>.Shared.Rent(s1Length + 1); // cost array, horizontally
        
        for (var i = 0; i <= s1Length; i++) 
            p[i] = i;

        for (var j = 1; j <= s2Length; j++)
        {
            var currentChar = s2[j - 1];
            d[0] = j;

            for (var i = 1; i <= s1Length; i++)
            {
                var cost = s1[i - 1] == currentChar ? 0 : 1;
                // minimum of cell to the left+1, to the top+1, diagonally left and up +cost				
                d[i] = Math.Min(Math.Min(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
            }

            // copy current distance counts to 'previous row' distance counts
            (p, d) = (d, p);
        }

        ArrayPool<int>.Shared.Return(p);
        ArrayPool<int>.Shared.Return(d);
        
        // our last action in the above loop was to switch d and p, so p now 
        // actually has the most recent cost counts
        return p[s1Length];
    }
}
