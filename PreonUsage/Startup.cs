using System.Collections.Generic;
using System.IO;
using System.Xml;
using BioCXml;
using Microsoft.Extensions.Options;
using PreonSharp;

namespace PreonUsage;

internal sealed class Startup(
    ILogger<Startup> logger,
    INormalizer normalizer)
{
    public async Task Run()
    {
        var xmlReaderSettings = new XmlReaderSettings()
        {
            DtdProcessing = DtdProcessing.Ignore,
        };

        await Task.WhenAll(
            Directory.GetFiles("corpora/nlm_gene", "*.XML")
                .Select(p => File.Open(p, new FileStreamOptions()))
                .Select(fs => XmlReader.Create(fs, xmlReaderSettings))
                .Select(reader => BioCXmlSerializerProvider.Deserialize(reader))
                .SelectMany(collection => collection.Document)
                .SelectMany(d => d.Passage)
                .SelectMany(p => p.Annotation)
                .Select(a => (a.Text,
                    NcbiId: a.Infon.FirstOrDefault(i => i.Key == "NCBI Gene identifier")?.Text))
                .Where(tuple => tuple.NcbiId != null)
                .Select(tuple => (tuple.Text, NcbiId: string.Join(" ", tuple.NcbiId!)))
                .Distinct()
                .Aggregate(new List<Task>(), (list, tuple) =>
                {
                    var (text, expectedId) = tuple;
                    list.Add(TestCase(expectedId, text));
                    return list;
                })
        );
    }

    private async Task TestCase(string expected, string text)
    {
        var queryResult = await normalizer.QueryAsync(text);
        var str = QueryResultJsonSerializerContext.Serialize(queryResult);
        logger.LogDebug("{}: expected {} got {}", text, expected, str);
    }
}