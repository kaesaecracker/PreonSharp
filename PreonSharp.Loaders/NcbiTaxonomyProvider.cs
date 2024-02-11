using System.IO;
using Microsoft.Extensions.Options;

namespace PreonSharp.Loaders;

internal sealed class NcbiTaxonomyProvider : IKnowledgeProvider
{
    private readonly ILogger<NcbiTaxonomyProvider> _logger;
    private readonly NcbiTaxonomyConfiguration _config;
    private readonly string _file;

    public NcbiTaxonomyProvider(ILogger<NcbiTaxonomyProvider> logger,
        IOptions<NcbiTaxonomyConfiguration> config)
    {
        _logger = logger;
        _config = config.Value;

        if (!File.Exists(_config.NamesDmpFile))
        {
            throw new FileNotFoundException(
                $"invalid {nameof(NcbiGeneTsvConfiguration.TsvFile)} configured",
                _config.NamesDmpFile);
        }

        _file = _config.NamesDmpFile;
    }

    public string SourceName => nameof(NcbiTaxonomyProvider);

    public async IAsyncEnumerable<(string, string)> GetNameIdPairs()
    {
        _logger.LogInformation("loading file {}", _file);
        using var reader = new StreamReader(_file);
        using var csv = new CsvReader(reader, _config.CsvReaderConfiguration);

        while (await csv.ReadAsync())
        {
            var name = csv.GetField(_config.NamesDmpNameColumn);
            if (string.IsNullOrWhiteSpace(name))
                continue;

            var id = csv.GetField(_config.NamesDmpIdColumn);
            yield return (name, id);
        }
    }
}
