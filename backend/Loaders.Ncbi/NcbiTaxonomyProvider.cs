using System.Collections.Frozen;
using System.Globalization;
using System.IO;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Taxonomy.Models;

namespace Loaders.Ncbi;

// https://www.nlm.nih.gov/research/umls/sourcereleasedocs/current/NCBI/sourcerepresentation.html

public sealed class NcbiTaxonomyProvider : BackgroundService
{
    private readonly string _dataRoot;
    private FrozenDictionary<ulong, TaxonomyEntity>? _entities;
    private readonly ILogger _logger;
    private readonly TaskCompletionSource _startCompletion = new();

    public NcbiTaxonomyProvider(IOptions<NcbiConfiguration> options, ILogger<NcbiTaxonomyProvider> logger)
    {
        _logger = logger;
        var config = options.Value;
        _dataRoot = config.TaxonomyDataRoot ?? throw new ConfigurationException("Ncbi DataRoot not specified");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("starting taxonomy provider");
        var hierarchy = await LoadNodes($"{_dataRoot}/nodes.dmp");
        _logger.LogDebug("loaded hierarchy");
        _entities = await LoadEntities($"{_dataRoot}/names.dmp", hierarchy);
        _logger.LogInformation("loaded {} entities", _entities.Count);
        _startCompletion.SetResult();
    }

    public IEnumerable<TaxonomyEntity> All => _entities?.Values
                                              ?? throw new InvalidOperationException("not initialized");

    public Task Started => _startCompletion.Task;

    private static async Task<Dictionary<ulong, ulong>> LoadNodes(string dmpFile)
    {
        using var csvReader = new CsvReader(
            new StreamReader(dmpFile),
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                HasHeaderRecord = false,
                Mode = CsvMode.NoEscape,
            });

        var result = new Dictionary<ulong, ulong>();
        while (await csvReader.ReadAsync())
        {
            var id = csvReader.GetField<ulong>(0);
            var parent = csvReader.GetField<ulong>(1);
            if (id == parent)
                continue;

            result[id] = parent;
        }

        return result;
    }

    private static async Task<FrozenDictionary<ulong, TaxonomyEntity>> LoadEntities(string dmpFile,
        Dictionary<ulong, ulong> hierarchy)
    {
        using var csvReader = new CsvReader(
            new StreamReader(dmpFile),
            new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|",
                HasHeaderRecord = false,
                Mode = CsvMode.NoEscape,
            });

        List<TaxonomyEntity> entities = [];
        List<EntityTag> names = [];
        ulong currentId = 1;
        while (await csvReader.ReadAsync())
        {
            var id = csvReader.GetField<ulong>(0);
            if (id != currentId)
            {
                ulong? parent;
                if (hierarchy.TryGetValue(currentId, out var foundParent))
                    parent = foundParent;
                else
                    parent = null;

                var entity = new TaxonomyEntity(currentId, names.ToFrozenSet(), parent);
                entities.Add(entity);
                names.Clear();
                currentId = id;
            }

            var name = csvReader[2];
            if (string.IsNullOrWhiteSpace(name))
                name = csvReader[1];
            name = name.Trim();

            names.Add(new EntityTag(csvReader[3].Trim(), name));
        }

        return entities.ToFrozenDictionary(e => e.TaxonomyId, e => e);
    }
}