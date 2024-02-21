using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace PreonSharp.Loaders;

public sealed class SepFileKnowledgeProvider : IKnowledgeProvider
{
    private readonly ILogger _logger;
    private readonly string _filePath;
    private readonly char _separator;
    private readonly int _idColumnIndex;
    private readonly int _nameColumnIndex;
    private readonly bool _hasHeader;
    private readonly bool _unquote;

    public SepFileKnowledgeProvider(SepFileSpec spec, ILogger<SepFileKnowledgeProvider> logger)
    {
        _logger = logger;
        _logger.LogDebug("spec: {}", spec);

        if (string.IsNullOrWhiteSpace(spec.FilePath))
            throw new ArgumentException(
                $"cannot create {nameof(SepFileKnowledgeProvider)} with path '{spec.FilePath}'");
        _filePath = spec.FilePath;

        if (spec.Separator == null)
            throw new ArgumentException($"no {nameof(spec.Separator)} specified for file '{spec.FilePath}'");
        _separator = spec.Separator.Value;

        if (spec.IdColumnIndex == null)
            throw new ArgumentException($"no {nameof(spec.IdColumnIndex)} specified for file '{spec.FilePath}'");
        _idColumnIndex = spec.IdColumnIndex.Value;

        if (spec.NameColumnIndex == null)
            throw new ArgumentException($"no {nameof(spec.NameColumnIndex)} specified for file '{spec.FilePath}'");
        _nameColumnIndex = spec.NameColumnIndex.Value;

        _hasHeader = spec.HasHeader;
        _unquote = spec.Unquote;
    }

    public IEnumerable<(string, string)> GetNameIdPairs()
    {
        _logger.LogInformation("opening file {}", _filePath);
        var csvReaderConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = _separator.ToString(),
            HasHeaderRecord = _hasHeader,
            Mode = _unquote ? CsvMode.RFC4180 : CsvMode.NoEscape,
        };

        var csvReader = new CsvReader(new StreamReader(_filePath), csvReaderConfig);
        if (_hasHeader)
        {
            csvReader.Read();
            csvReader.ReadHeader();
        }

        return StreamRead(csvReader);
    }

    private IEnumerable<(string, string)> StreamRead(CsvReader csvReader)
    {
        while (csvReader.Read())
        {
            var name = csvReader[_nameColumnIndex];
            if (string.IsNullOrWhiteSpace(name))
                continue; // no name

            name = name.Trim();
            var id = csvReader[_idColumnIndex].Trim();
            yield return (name, id);
        }

        csvReader.Dispose();
    }

    public override string ToString()
    {
        return $"{nameof(SepFileKnowledgeProvider)} ({_filePath})";
    }
}