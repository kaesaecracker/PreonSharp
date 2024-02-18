using System.IO;
using System.Xml;
using BioCXml;
using PreonSharp;

namespace PreonUsage;

internal sealed class QueryGenerator(
    ILogger<QueryGenerator> logger,
    INormalizer normalizer)
{
    public async Task Run(DirectoryInfo directory)
    {
        var tasks = directory
            .GetFiles("*.XML")
            .Select(LoadCorpus)
            .SelectMany(ExtractAnnotations)
            .SelectMany(a => ExtractNameIdPairs(a.Text, a.Infon))
            .Distinct()
            .Select(GenerateTestCaseTask);
        await Task.WhenAll(tasks);
    }

    private async Task GenerateTestCaseTask((string Text, string Id) tuple)
    {
        var result = await normalizer.QueryAsync(tuple.Text);
        logger.LogDebug("{}: expected {} got {}", tuple.Text, tuple.Id, result);
    }

    private static Collection LoadCorpus(FileInfo file)
    {
        var reader = XmlReader.Create(file.OpenRead(), new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Ignore
        });
        return BioCXmlSerializerProvider.Deserialize(reader);
    }

    private static IEnumerable<Annotation> ExtractAnnotations(Collection collection)
    {
        // TODO: there are more annotations on other objects
        return collection.Document
            .SelectMany(d => d.Passage)
            .SelectMany(p => p.Annotation);
    }

    private static IEnumerable<(string Text, string Id)> ExtractNameIdPairs(string text, List<Infon> infons)
    {
        if (string.IsNullOrWhiteSpace(text))
            yield break;

        var ids = infons
            .Where(i => i.Key == "NCBI Gene identifier")
            .Select(i => i.Text.Aggregate((a, b) => a + b));

        foreach (var id in ids)
            yield return (text, id);
    }
}