using System.Text.Json.Serialization;

namespace Taxonomy;

[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(Entity))]
[JsonSerializable(typeof(EntityTag))]
[JsonSerializable(typeof(EntitySource))]
[JsonSerializable(typeof(IdNamespace))]
[JsonSerializable(typeof(TextMatch))]
[JsonSerializable(typeof(UnifiedSearchResult))]
[JsonSerializable(typeof(UnifiedSearchResultKind))]
[JsonSerializable(typeof(IEnumerable<Entity>))]
[JsonSerializable(typeof(IEnumerable<EntityTag>))]
[JsonSerializable(typeof(IEnumerable<TextMatch>))]
[JsonSerializable(typeof(ISet<TextMatch>))]
public partial class TaxonomyJsonSerializerContext : JsonSerializerContext;
