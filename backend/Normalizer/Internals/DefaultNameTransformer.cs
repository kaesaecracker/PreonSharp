using System.Text.RegularExpressions;

namespace Normalizer.Internals;

public partial class DefaultNameTransformer : INameTransformer
{
    [GeneratedRegex("[^a-zA-Z0-9]")]
    private static partial Regex DeleteCharsRegex();

    public string Transform(string name)
        => DeleteCharsRegex().Replace(name.ToLowerInvariant(), string.Empty);
}
