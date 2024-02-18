using System.Text.Json;
using System.Text.Json.Serialization;

namespace PreonSharp;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    GenerationMode = JsonSourceGenerationMode.Serialization,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(QueryResult))]
[JsonConverter(typeof(JsonStringEnumConverter<MatchType>))]
[JsonSerializable(typeof(MatchType))]
internal sealed partial class QueryResultJsonSerializerContext : JsonSerializerContext
{
    public static string Serialize(QueryResult? result)
    {
        return result != null
            ? JsonSerializer.Serialize(result, Default.QueryResult)
            : "(null)";
    }
}