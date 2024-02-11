using PreonSharp;
using PreonSharp.Loaders;

namespace PreonUsage;

internal sealed class Program
{
    public static async Task Main()
    {
        var services = ConfigureServices();
        await services.GetRequiredService<Startup>().Run();
    }

    private static ServiceProvider ConfigureServices() => new ServiceCollection()
        .AddSingleton<Startup>()
        .AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Debug);
            builder.AddSimpleConsole(options =>
            {
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss.ffff ";
                options.IncludeScopes = true;
            });
        })
        .AddNormalizer(builder => builder
            .AddExactMatchStrategy()
            .AddLevenshteinMatchStrategy()
            .AddNcbiTaxonomy(c => { c.NamesDmpFile = "ncbi/names.dmp"; })
            .AddNcbiGeneTsv(c =>
            {
                c.TsvFile = "ncbi/gene_info.tsv";
                c.SkipRows = 10;
            }))
        .BuildServiceProvider();
}
