using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PreonSharp.Levenshtein;
using PreonSharp.Loaders;

namespace PreonSharpWebApi;

internal static class Program
{
    public static void Main(string[] args)
    {
        var app = BuildWebApp(args);
        app.UseHttpLogging();

        var normalizer = app.Services.GetRequiredService<INormalizer>();

        var preonApi = app.MapGroup("/preon")
            .WithOpenApi();
        preonApi.MapGet("/", (string s) => normalizer.QueryAsync(s))
            .WithOpenApi();
        preonApi.MapGet("/wait", normalizer.WaitForInitializationAsync)
            .WithOpenApi();

        app.Run();
    }

    private static WebApplication BuildWebApp(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Configuration
            .AddCommandLine(args)
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

        builder.Logging.AddSimpleConsole(options =>
        {
            options.SingleLine = false;
            options.TimestampFormat = "HH:mm:ss.ffff ";
            options.IncludeScopes = true;
        });

        builder.Services.AddHttpLogging(_ => { });
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddNormalizer(normalizerBuilder =>
        {
            normalizerBuilder.AddLevenshteinMatchStrategy();
            normalizerBuilder.AddSepFiles();
        });

        builder.Services.Configure<SepFilesConfiguration>(
            builder.Configuration.GetSection("SepFiles"));

        return builder.Build();
    }
}