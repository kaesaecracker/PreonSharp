using System.IO;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace PreonSharp.Loaders;

internal sealed class GenericCsvLoader : IAsyncEnumerator<string[]>
{
    private readonly StreamReader _streamReader;
    private readonly CsvReader _csvReader;
    private readonly int[] _columns;
    private readonly int _skipRows;

    public GenericCsvLoader(IReaderConfiguration csvReaderConfiguration, int[] columns, string? dataFile,
        int skipRows = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataFile);
        _columns = columns;
        _skipRows = skipRows;
        _streamReader = new StreamReader(dataFile);
        _csvReader = new CsvReader(_streamReader, csvReaderConfiguration);
        Current = new string[_columns.Length];

        if (csvReaderConfiguration.HasHeaderRecord)
        {
            _csvReader.Read();
            _csvReader.ReadHeader();
        }
    }

    public string[] Current { get; }

    public async ValueTask<bool> MoveNextAsync()
    {
        var skipped = 0;
        do
        {
            if (!await _csvReader.ReadAsync())
                return false;
        } while (skipped++ < _skipRows);

        for (var i = 0; i < _columns.Length; i++)
            Current[i] = _csvReader.GetField(_columns[i]).Trim();

        return true;
    }

    public ValueTask DisposeAsync()
    {
        _csvReader.Dispose();
        _streamReader.Dispose();
        return ValueTask.CompletedTask;
    }
}
