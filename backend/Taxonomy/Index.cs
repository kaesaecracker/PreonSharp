using System.Collections.Frozen;
using Normalizer;

namespace Taxonomy;

public abstract class Index(IEntityProvider entityProvider, INameTransformer nameTransformer)
{
    private FrozenDictionary<string, FrozenSet<Entity>>? _dict = null;

    public async Task BuildAsync()
    {
        await entityProvider.Started;

        Dictionary<string, HashSet<Entity>> dict = new();
        foreach (var entity in entityProvider.All)
        foreach (var str in Selector(entity))
        {
            var transformed = string.Intern(nameTransformer.Transform(str));
            if (dict.TryGetValue(transformed, out var entityList))
                entityList.Add(entity);
            else
                dict[transformed] = [entity];
        }

        _dict = dict.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToFrozenSet());
    }

    protected abstract IEnumerable<string> Selector(Entity entity);

    public IReadOnlySet<Entity>? GetExactMatch(string text)
    {
        if (_dict == null)
            throw new InvalidOperationException();
        return _dict.GetValueOrDefault(text);
    }
}