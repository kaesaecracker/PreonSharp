using System.Text.Json.Serialization;
using Loaders.Ncbi;

namespace PreonSharpWebApi;

[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(QueryResult))]
[JsonConverter(typeof(JsonStringEnumConverter<MatchType>))]
[JsonSerializable(typeof(MatchType))]
[JsonSerializable(typeof(TaxonomyEntity))]
public partial class AppJsonSerializerContext : JsonSerializerContext;