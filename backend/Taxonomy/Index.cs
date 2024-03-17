using System.Collections.Frozen;

namespace Taxonomy;

public abstract class Index(IEntityProvider entityProvider)
{
    private FrozenDictionary<string, FrozenSet<Entity>>? _dict = null;

    public async Task BuildAsync()
    {
        await entityProvider.Started;

        Dictionary<string, HashSet<Entity>> dict = new();
        foreach (var entity in entityProvider.All)
        foreach (var str in Selector(entity))
        {
            if (dict.TryGetValue(str, out var entityList))
                entityList.Add(entity);
            else
                dict[str] = [entity];
        }

        _dict = dict.ToFrozenDictionary(kvp => kvp.Key, kvp => kvp.Value.ToFrozenSet());
    }

    protected abstract IEnumerable<string> Selector(Entity entity);

    public IReadOnlySet<Entity>? GetExactMatch(string text)
    {
        if (_dict == null)
            throw new InvalidOperationException();
        return _dict.GetValueOrDefault(text);
    }
}