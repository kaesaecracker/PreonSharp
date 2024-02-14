using System.Numerics;
using System.Runtime.CompilerServices;

namespace Levenshtein;

internal static class MathExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(T a, T b) where T : struct, IComparisonOperators<T, T, bool>
        => a < b ? a : b;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(T a, T b, T c) where T : struct, IComparisonOperators<T, T, bool>
        => Min(a, Min(b, c));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Min<T>(T a, T b, T c, T d) where T : struct, IComparisonOperators<T, T, bool>
        => Min(Min(a, b), Min(b, c));
}