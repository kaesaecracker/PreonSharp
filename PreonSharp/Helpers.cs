using System.Text.RegularExpressions;

namespace PreonSharp;

public static class Helpers
{
    public static IEnumerable<T[]> Ngrams<T>(IEnumerable<T> sequence, int n)
    {
        List<T> history = new(n);
        foreach (var item in sequence)
        {
            history.Add(item);
            if (history.Count < n)
                continue;
            yield return history.ToArray();
            history.RemoveAt(0);
        }
    }
}
