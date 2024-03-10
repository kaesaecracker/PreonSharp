using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Loaders.Ncbi;

// https://www.nlm.nih.gov/research/umls/sourcereleasedocs/current/NCBI/sourcerepresentation.html

public sealed class TaxonomyProvider : IHostedLifecycleService
{
    private readonly string _dataRoot;
    private FrozenDictionary<ulong, TaxonomyEntity>? _entities;
    private readonly ILogger _logger;
    private Task? _startTask;

    public TaxonomyProvider(IOptions<NcbiConfiguration> options, ILogger<TaxonomyProvider> logger)
    {
        _logger = logger;
        var config = options.Value;
        _dataRoot = config.TaxonomyDataRoot ?? throw new ConfigurationException("Ncbi DataRoot not specified");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _startTask = Task.Run(() =>
        {
            _logger.LogInformation("starting taxonomy provider");
            var hierarchy = LoadNodes($"{_dataRoot}/nodes.dmp");
            _entities = LoadEntities($"{_dataRoot}/names.dmp", hierarchy);
            _logger.LogInformation("loaded {} entities", _entities.Count);
        }, cancellationToken);
        return _startTask;
    }

    public Task StartedAsync(CancellationToken cancellationToken) => _startTask ?? Task.CompletedTask;

    public TaxonomyEntity? GetEntity(ulong id)
    {
        if (_entities?.TryGetValue(id, out var result) ?? false)
            return result;
        return null;
    }
    
    private static Dictionary<ulong, ulong> LoadNodes(string dmpFile)
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
        while (csvReader.Read())
        {
            var id = csvReader.GetField<ulong>(0);
            var parent = csvReader.GetField<ulong>(1);
            if (id == parent)
                continue;
            
            result[id] = parent;
        }

        return result;
    }

    private static FrozenDictionary<ulong, TaxonomyEntity> LoadEntities(string dmpFile, Dictionary<ulong, ulong> hierarchy)
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
        List<TaxonomyTag> names = [];
        ulong currentId = 1;
        while (csvReader.Read())
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
                
            names.Add(new TaxonomyTag(csvReader[3].Trim(), name));
        }

        return entities.ToFrozenDictionary(e => e.TaxonomyId, e => e);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}