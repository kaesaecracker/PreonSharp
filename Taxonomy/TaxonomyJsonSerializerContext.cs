using System.Text.Json.Serialization;

namespace Taxonomy;

[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(Entity))]
[JsonSerializable(typeof(EntityTag))]
[JsonSerializable(typeof(EntitySource))]
[JsonSerializable(typeof(Source))]
[JsonSerializable(typeof(IEnumerable<Entity>))]
[JsonSerializable(typeof(IEnumerable<EntityTag>))]
public partial class TaxonomyJsonSerializerContext : JsonSerializerContext;