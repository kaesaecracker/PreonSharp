using System.IO;
using PreonSharp;
using PreonSharp.Loaders;
using PreonSharp.Levenshtein;

namespace PreonUsage;

internal static class Program
{
    public static async Task Main()
    {
        var services = ConfigureServices();
        await services.GetRequiredService<QueryGenerator>().Run(new DirectoryInfo("corpora/nlm_gene"));
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<QueryGenerator>();

        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Debug);
            builder.AddSimpleConsole(options =>
            {
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss.ffff ";
                options.IncludeScopes = true;
            });
        });

        services.AddNormalizer(builder =>
        {
            builder.AddLevenshteinMatchStrategy();
            //builder.AddFastenshteinMatchStrategy();

            builder.AddSepFile(new SepFileSpec
            {
                FilePath = "ncbi/names.dmp",
                Separator = '|',
                HasHeader = false,
                NameColumnIndex = 1,
                IdColumnIndex = 0,
            });

            builder.AddSepFile(new SepFileSpec
            {
                FilePath = "ncbi/gene_info.tsv",
                Separator = '\t',
                HasHeader = true,
                NameColumnIndex = 8,
                IdColumnIndex = 2,
            });

            foreach (var file in Directory.GetFiles("ebi", "*.tsv"))
            {
                builder.AddSepFile(new SepFileSpec
                {
                    FilePath = file,
                    Separator = '\t',
                    HasHeader = true,
                    NameColumnIndex = 1,
                    IdColumnIndex = 0,
                });
            }
        });

        return services.BuildServiceProvider();
    }
}