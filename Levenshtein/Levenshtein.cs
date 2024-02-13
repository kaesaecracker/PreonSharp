using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Levenshtein;

internal interface ILevenshteinCosts
{
    decimal GetInsertCost(char c);
    decimal GetDeleteCost(char c);
    decimal GetReplaceCost(char from, char to);
}

internal sealed class UniformLevenshteinCosts : ILevenshteinCosts
{
    public decimal GetInsertCost(char c) => 1;
    public decimal GetDeleteCost(char c) => 1;
    public decimal GetReplaceCost(char from, char to) => 1;
}

internal sealed class DistanceMatrix
{
    private readonly List<decimal> _d = [];

    public void ResizeAndReset(int rows, int columns)
    {
        _d.Clear();

        var items = rows * columns;
        _d.EnsureCapacity(items);

        for (var i = 0; i < items; i++)
            _d.Add(0m);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ToIndex(int row, int column) => row * column + column;

    public decimal this[int row, int column]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _d[ToIndex(row, column)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _d[ToIndex(row, column)] = value;
    }
}

internal sealed class Levenshtein
{
    private readonly ConcurrentBag<DistanceMatrix> _distanceMatrixPool = [];

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public decimal CalculateDistance(string s1, string s2)
    {
        // TODO: if s1 or s2 is empty, we do not need to allocate an array

        if (!_distanceMatrixPool.TryTake(out var d))
            d = new DistanceMatrix();
        d.ResizeAndReset(s1.Length + 1, s2.Length + 1);

        // Init the matrix
        for (var i = 0; i <= s1.Length; i += 1)
            d[i, 0] = i;
        for (var j = 0; j <= s2.Length; j += 1)
            d[0, j] = j;

        for (var i = 1; i <= s1.Length; i += 1)
        for (var j = 1; j <= s2.Length; j += 1)
        {
            var delCost = d[i - 1, j] + 1;
            var insCost = d[i, j - 1] + 1;
            var subCost = d[i - 1, j - 1] + 1;

            d[i, j] = decimal.Min(delCost, decimal.Min(insCost, subCost));
        }

        var result = d[s1.Length, s2.Length];
        _distanceMatrixPool.Add(d);
        return result;
    }
}
