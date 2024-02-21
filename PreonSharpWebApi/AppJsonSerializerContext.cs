using System.Text.Json.Serialization;

namespace PreonSharpWebApi;

[JsonSourceGenerationOptions]
[JsonSerializable(typeof(QueryResult))]
[JsonConverter(typeof(JsonStringEnumConverter<MatchType>))]
[JsonSerializable(typeof(MatchType))]
public partial class AppJsonSerializerContext : JsonSerializerContext;