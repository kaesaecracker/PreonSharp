using System.Text.Json.Serialization;
using PreonSharp;

namespace PreonUsage;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(QueryResult))]
internal sealed partial class QueryResultJsonSerializerContext : JsonSerializerContext
{
}