namespace PreonSharp.Loaders;

internal sealed class NcbiGeneTsvKnowledgeProvider : IKnowledgeProvider
{
    private readonly NcbiGeneTsvConfiguration _config;
    private readonly ILogger<NcbiGeneTsvKnowledgeProvider> _logger;

    public NcbiGeneTsvKnowledgeProvider(IOptions<NcbiGeneTsvConfiguration> config,
        ILogger<NcbiGeneTsvKnowledgeProvider> logger)
    {
        _logger = logger;
        _config = config.Value;
        ArgumentNullException.ThrowIfNull(_config.TsvFile);
    }

    public string SourceName => nameof(NcbiGeneTsvKnowledgeProvider);

    public async IAsyncEnumerable<(string, string)> GetNameIdPairs()
    {
        _logger.LogInformation("opening file {}", _config.TsvFile);
        await using var loader = new GenericCsvLoader(
            _config.CsvReaderConfiguration,
            [_config.NameColumn, _config.IdColumn],
            _config.TsvFile,
            _config.SkipRows
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
