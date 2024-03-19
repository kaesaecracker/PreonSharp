using System.Text.Json.Serialization;
using Normalizer;

namespace WebApi;

[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(QueryResult))]
[JsonConverter(typeof(JsonStringEnumConverter<MatchType>))]
[JsonSerializable(typeof(MatchType))]
public partial class AppJsonSerializerContext : JsonSerializerContext;