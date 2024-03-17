using Loaders.Ncbi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PreonSharp.Levenshtein;
using Loaders.SepFiles;
using Microsoft.Extensions.Hosting;

namespace PreonSharpWebApi;

internal static class Program
{
    public static void Main(string[] args)
    {
        var app = BuildWebApp(args);
        app.UseHttpLogging();
        app.UseCors();

        app.MapGet("/ping", () => "pong");

        var normalizer = app.Services.GetRequiredService<INormalizer>();
        var preonApi = app.MapGroup("/preon");
        preonApi.MapGet("/query", (string s) => normalizer.QueryAsync(s));
        preonApi.MapGet("/wait", normalizer.WaitForInitializationAsync);

        var taxonomyProvider = app.Services.GetRequiredService<TaxonomyProvider>();
        var taxonomyApi = app.MapGroup("/taxonomy");
        taxonomyApi.MapGet("/{id}", (ulong id) => taxonomyProvider.GetEntity(id));

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
            });

        builder.Services
            .AddNormalizer(normalizerBuilder =>
            {
                normalizerBuilder.AddLevenshteinMatchStrategy();
                normalizerBuilder.AddSepFiles();
            })
            .Configure<SepFilesConfiguration>(builder.Configuration.GetSection("SepFiles"))
            .AddSingleton<TaxonomyProvider>()
            .Configure<NcbiConfiguration>(builder.Configuration.GetSection("Ncbi"))
            .AddHostedService(sp => sp.GetRequiredService<TaxonomyProvider>());

        return builder.Build();
    }
}