using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using PreonSharp;
using PreonSharp.Loaders;

var services = ConfigureServices();

var logger = services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("starting up");

var normalizer = services.GetRequiredService<INormalizer>();
logger.LogInformation("loaded {} names", normalizer.NameCount);

string[] testQueries = ["test", "CAMDRONATEE", "CAMDRONATE", "COVID-19"];
var allResults = testQueries.ToDictionary(s => s, s => normalizer.Query(s));

var prettyPrint = new JsonSerializerOptions()
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = true,
};
prettyPrint.Converters.Add(new JsonStringEnumConverter());
logger.LogInformation("results: {}", JsonSerializer.Serialize(allResults, prettyPrint));

return;

ServiceProvider ConfigureServices()
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
        builder.AddEbiCsv("ebi-compounds.tsv");
    });

    return serviceCollection.BuildServiceProvider();
}
