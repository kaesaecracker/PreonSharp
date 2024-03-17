using Loaders.Ncbi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Loaders.SepFiles;
using Microsoft.Extensions.Hosting;
using Normalizer;
using Normalizer.Levenshtein;
using Taxonomy;

namespace WebApi;

internal static class Program
{
    public static void Main(string[] args)
    {
        var app = BuildWebApp(args);
        app.UseHttpLogging();
        app.UseCors();

        app.MapGet("/ping", () => "pong");

        app.Services.GetRequiredService<TaxonomyEndpoints>().Map(app.MapGroup("/taxonomy"));
        app.Services.GetRequiredService<NormalizerEndpoints>().Map(app.MapGroup("/normalizer"));

        app.Run();
    }

    private static WebApplication BuildWebApp(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Host.ConfigureHostOptions(options =>
        {
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = true;
        });

        builder.Configuration
            .AddCommandLine(args)
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

        builder.Logging
            .AddSimpleConsole(options =>
            {
                options.SingleLine = false;
                options.TimestampFormat = "HH:mm:ss.ffff ";
                options.IncludeScopes = true;
            });

        builder.Services
            .AddCors(options => options.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder.AllowAnyOrigin();
                policyBuilder.AllowAnyHeader();
            }))
            .AddHttpLogging(_ => { })
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
                options.SerializerOptions.TypeInfoResolverChain.Insert(1, TaxonomyJsonSerializerContext.Default);
            });

        builder.Services
            .AddNcbiTaxonomy()
            .AddSingleton<TaxonomyEndpoints>()
            .AddSingleton<NormalizerEndpoints>()
            .Configure<NcbiConfiguration>(builder.Configuration.GetSection("Ncbi"))
            .Configure<SepFilesConfiguration>(builder.Configuration.GetSection("SepFiles"))
            .AddTaxonomy(taxonomyBuilder =>
            {
                taxonomyBuilder.AddNcbiEntityLoader();
            })
            .AddNormalizer(normalizerBuilder =>
            {
                normalizerBuilder.AddLevenshteinMatchStrategy();
                normalizerBuilder.AddSepFiles();
                normalizerBuilder.AddNcbiTaxonomyKnowledge();
            });

        return builder.Build();
    }
}