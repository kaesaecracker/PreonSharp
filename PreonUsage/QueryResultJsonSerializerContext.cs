using System.Text.Json;
using System.Text.Json.Serialization;
using PreonSharp;

namespace PreonUsage;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(QueryResult))]
internal sealed partial class QueryResultJsonSerializerContext : JsonSerializerContext
{
    public static string? Serialize(QueryResult? result)
    {
        return result != null
            ? JsonSerializer.Serialize(result, Default.QueryResult)
            : null;
    }
}