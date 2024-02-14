using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using PreonSharp;

namespace PreonUsage;

internal sealed class Startup
{
    private readonly JsonSerializerOptions _prettyPrint;
    private readonly ILogger _logger;
    private readonly INormalizer _normalizer;

    public Startup(ILogger<Startup> logger, INormalizer normalizer)
    {
        _prettyPrint = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };
        _prettyPrint.Converters.Add(new JsonStringEnumConverter());

        _logger = logger;
        _normalizer = normalizer;
    }

    public async Task Run()
    {
        var serializer = new XmlSerializer(typeof(BioCXml.Collection));
        var xmlReaderSettings = new XmlReaderSettings()
        {
            DtdProcessing = DtdProcessing.Ignore,
        };

        var result = Directory.GetFiles("corpora/nlm_gene", "*.XML")
            .Select(p => File.Open(p, new FileStreamOptions()))
            .Select(fs => XmlReader.Create(fs, xmlReaderSettings))
            .Select(reader => (BioCXml.Collection)serializer.Deserialize(reader)!)
            .SelectMany(collection => collection.Document)
            .SelectMany(d => d.Passage)
            .SelectMany(p => p.Annotation)
            .Select(a => (a.Text,
                NcbiId: a.Infon.FirstOrDefault(i => i.Key == "NCBI Gene identifier")?.Text))
            .Where(tuple => tuple.NcbiId != null)
            .Select(tuple => (tuple.Text, NcbiIdentifier: string.Join(" ", tuple.NcbiId!)))
            .Distinct()
            .Take(50);

        await Parallel.ForEachAsync(result, new ParallelOptions
        {
            MaxDegreeOfParallelism = 1,//Math.Max(2, Environment.ProcessorCount / 4),
        }, async (tuple, token) =>
        {
            var (text, expectedId) = tuple;
            QueryResult? queryResult = await _normalizer.QueryAsync(text);
            _logger.LogInformation("{}: expected {} got {}", text, expectedId,
                JsonSerializer.Serialize(queryResult, _prettyPrint));
        });
    }
}
