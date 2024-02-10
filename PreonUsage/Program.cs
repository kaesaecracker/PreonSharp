using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using PreonSharp;
using PreonSharp.Loaders;

var services = ConfigureServices();

var logger = services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("starting up");

var normalizer = services.GetRequiredService<INormalizer>();
logger.LogInformation("loaded {} names", normalizer.NameCount);


var prettyPrint = new JsonSerializerOptions()
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = true,
};
prettyPrint.Converters.Add(new JsonStringEnumConverter());

string[] testQueries = ["test", "CAMDRONATEE", "CAMDRONATE", "COVID-19"];
var allResults = testQueries.ToDictionary(s => s, s => normalizer.Query(s));
logger.LogInformation("results: {}", JsonSerializer.Serialize(allResults, prettyPrint));

return;

static ServiceProvider ConfigureServices()
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

    return serviceCollection.BuildServiceProvider();
}
