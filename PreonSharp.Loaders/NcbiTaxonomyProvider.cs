namespace PreonSharp.Loaders;

internal sealed class NcbiTaxonomyProvider : IKnowledgeProvider
{
    private readonly NcbiTaxonomyConfiguration _config;
    private readonly ILogger<NcbiTaxonomyProvider> _logger;

    public NcbiTaxonomyProvider(ILogger<NcbiTaxonomyProvider> logger,
        IOptions<NcbiTaxonomyConfiguration> config)
    {
        _logger = logger;
        _config = config.Value;
        ArgumentNullException.ThrowIfNull(_config.NamesDmpFile);
    }

    public string SourceName => nameof(NcbiTaxonomyProvider);

    public async IAsyncEnumerable<(string, string)> GetNameIdPairs()
    {
        _logger.LogInformation("opening file {}", _config.NamesDmpFile);
        await using var loader = new GenericCsvLoader(
            _config.CsvReaderConfiguration,
            [_config.NamesDmpNameColumn, _config.NamesDmpIdColumn],
            _config.NamesDmpFile!
        );

        while (await loader.MoveNextAsync())
        {
            var row = loader.Current;
            if (string.IsNullOrWhiteSpace(row[0]))
                continue; // no name

            yield return (row[0], row[1]);
        }
    }
}
