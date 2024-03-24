using System.Collections.Frozen;
using Taxonomy.Internals;

namespace Taxonomy.Indexes;

public abstract class EntityIndex(IEntityProvider entityProvider, INameTransformer nameTransformer, ILogger logger)
{
    private FrozenDictionary<string, FrozenSet<Guid>>? _dict = null;

    public async Task BuildAsync()
    {
        await entityProvider.Started;

        Dictionary<string, HashSet<Guid>> dict = new();
        foreach (var entity in entityProvider.All)
        foreach (var str in Selector(entity))
        {
            var transformed = nameTransformer.Transform(str);
            if (dict.TryGetValue(transformed, out var entityList))
                entityList.Add(entity.Id);
            else
                dict[transformed] = [entity.Id];
        }

        _dict = dict.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToFrozenSet());
        logger.LogDebug("built index with {} text entries", _dict.Count);
    }

    protected abstract IEnumerable<string> Selector(Entity entity);

    public IReadOnlySet<Guid>? GetExactMatches(string text)
    {
        if (_dict == null)
            throw new InvalidOperationException();

        return _dict.GetValueOrDefault(text);
    }

    public ISet<TextMatch> GetClosestMatches(string text)
    {
        if (_dict == null)
            throw new InvalidOperationException();

        var transformedName = nameTransformer.Transform(text);
        var minDist = int.MaxValue;
        HashSet<TextMatch> minDistValues = [];

        var distObj = new LevenshteinSearch(transformedName);

        foreach (var (otherName, entities) in _dict)
        {
            var distance = distObj.DistanceFrom(otherName);
            if (distance < minDist)
            {
                minDistValues.Clear();
                minDist = distance;
            }

            if (distance == minDist)
                minDistValues.Add(new TextMatch(otherName, entities));
        }

        if (minDist == int.MaxValue)
            return new HashSet<TextMatch>();

        // var relativeDistance = minDist / (decimal)transformedName.Length;
        return minDistValues;
    }
}