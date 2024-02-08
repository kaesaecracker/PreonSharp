using System.Text.RegularExpressions;

namespace PreonSharp;

public partial class Helpers
{
    [GeneratedRegex("[^a-zA-Z0-9]")] 
    private static partial Regex DeleteCharsRegex();
    
    public static string TransformName(string name) 
        => DeleteCharsRegex().Replace(name.ToLowerInvariant(), string.Empty);

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