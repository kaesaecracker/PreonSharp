using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using BioCXml;
using Microsoft.Extensions.Logging;

namespace WebApi;

internal sealed class QueryGenerator(
    ILogger<QueryGenerator> logger,
    INormalizer normalizer)
{
    public async Task Run(DirectoryInfo directory)
    {
        var tasks = directory
            .GetFiles("*.XML")
            .Select(LoadCorpus)
            .SelectMany(c => c.Document)
            .SelectMany(d => d.Passage)
            .SelectMany(ExtractAnnotations)
            .SelectMany(ExtractNameIdPairs)
            .Distinct()
            .Select(GenerateTestCaseTask)
            .ToList();

        var allDoneTask = Task.WhenAll(tasks);
        while (!allDoneTask.IsCompleted)
        {
            logger.LogDebug("waiting for {} tasks", tasks.Count(t => !t.IsCompleted));
            await Task.Delay(1000);
        }

        await allDoneTask;
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

    private static IEnumerable<Annotation> ExtractAnnotations(Passage passage)
    {
        foreach (var annotation in passage.Annotation)
            yield return annotation;
        foreach (var annotation in passage.Sentence.SelectMany(s => s.Annotation))
            yield return annotation;
    }

    private static IEnumerable<(string Text, string Id)> ExtractNameIdPairs(Annotation a)
    {
        if (string.IsNullOrWhiteSpace(a.Text))
            yield break;

        var ids = a.Infon
            .Where(i => i.Key == "NCBI Gene identifier")
            .Select(i => string.Join(string.Empty, i.Text));

        foreach (var id in ids)
            yield return (a.Text, id);
    }
}