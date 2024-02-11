using System.IO;
using Microsoft.Extensions.Options;

namespace PreonSharp.Loaders;

internal sealed class NcbiGeneTsvKnowledgeProvider : IKnowledgeProvider
{
    private readonly NcbiGeneTsvConfiguration _config;
    private readonly ILogger<NcbiGeneTsvKnowledgeProvider> _logger;
    private readonly string _tsvFile;

    public NcbiGeneTsvKnowledgeProvider(IOptions<NcbiGeneTsvConfiguration> config,
        ILogger<NcbiGeneTsvKnowledgeProvider> logger)
    {
        _logger = logger;
        _config = config.Value;

        if (!File.Exists(_config.TsvFile))
        {
            throw new FileNotFoundException(
                $"invalid {nameof(NcbiGeneTsvConfiguration.TsvFile)} configured",
                _config.TsvFile);
        }

        _tsvFile = _config.TsvFile;
    }

    public string SourceName => nameof(NcbiGeneTsvKnowledgeProvider);

    public async IAsyncEnumerable<(string, string)> GetNameIdPairs()
    {
        _logger.LogInformation("loading file {}", _config.TsvFile);
        using var reader = new StreamReader(_tsvFile);
        using var csv = new CsvReader(reader, _config.CsvReaderConfiguration);

        await csv.ReadAsync();
        csv.ReadHeader();
        while (await csv.ReadAsync())
        {
            var name = csv.GetField(_config.NameColumn);
            if (string.IsNullOrWhiteSpace(name))
                continue;

            var id = csv.GetField(_config.IdColumn);
            yield return (name, id);

            for (var i = 0; i < _config.TestingSubset; i++)
                await csv.ReadAsync();
        }
    }
}
