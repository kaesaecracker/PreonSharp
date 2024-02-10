using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using PreonSharp;
using PreonSharp.Loaders;

namespace PreonUsage;

internal sealed class Program
{
    public static void Main(string[] args) => new Program().Run();

    private Program()
    {
        _prettyPrint = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };
        _prettyPrint.Converters.Add(new JsonStringEnumConverter());

        _services = ConfigureServices();
    }

    private readonly JsonSerializerOptions _prettyPrint;
    private readonly IServiceProvider _services;

    private void Run()
    {
        var logger = _services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("starting up");

        var normalizer = _services.GetRequiredService<INormalizer>();

        string[] testQueries = ["test", "CAMDRONATEE", "CAMDRONATE", "COVID-19"];
        var allResults = testQueries.ToDictionary(s => s, s => normalizer.Query(s));
        logger.LogInformation("results: {}", JsonSerializer.Serialize(allResults, _prettyPrint));
    }

    private static ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Debug);
            builder.AddSimpleConsole(options =>
            {
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss ";
            });
        });

    serviceCollection.AddNormalizer(builder =>
    {
        foreach (var path in Directory.GetFiles("ebi", "*.tsv"))
            builder.AddEbiTsv(path);
    });
        serviceCollection.AddNormalizer(builder =>
        {
            foreach (var path in Directory.GetFiles("ebi", "*.tsv"))
                builder.AddEbiTsv(path);

            builder.AddNcbiGeneTsv("ncbi/gene_info.tsv");
        });

        return serviceCollection.BuildServiceProvider();
    }
}
