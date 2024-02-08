using System.Collections.Frozen;
using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp;

public class PrecisionOncologyNormalizerBuilder(IServiceProvider services)
{
    private readonly ILogger _logger = services.GetRequiredService<ILogger<PrecisionOncologyNormalizerBuilder>>();
    private readonly Dictionary<string, HashSet<string>> _normalizedNames = new();

    public PrecisionOncologyNormalizerBuilder Fit(string[] names, string[] ids)
    {
        if (names.Length != ids.Length)
            throw new ArgumentException($"length of {nameof(names)} and {nameof(ids)} must be equal");

        foreach (var (name, id) in names.Zip(ids))
            Fit(name, id);

        return this;
    }

    public PrecisionOncologyNormalizerBuilder Fit(string name, string id)
    {
        var transformedName = Helpers.TransformName(name);
        _logger.LogTrace("fitting name {} to id {} with transformation {}", name, id, transformedName);
        if (_normalizedNames.TryGetValue(transformedName, out var existingIds))
            existingIds.Add(id);
        else
            _normalizedNames.Add(transformedName, [id]);
        return this;
    }

    public IPrecisionOncologyNormalizer Build()
    {
        var normalizerLogger = services.GetRequiredService<ILogger<PrecisionOncologyNormalizer>>();
        var readOnlyNames = _normalizedNames
            .Select(kvp => new KeyValuePair<string, IReadOnlySet<string>>(kvp.Key, kvp.Value.ToFrozenSet()))
            .ToFrozenDictionary();
        return new PrecisionOncologyNormalizer(readOnlyNames, normalizerLogger);
    }
}
