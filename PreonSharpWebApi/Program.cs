using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Preon.WebApi.Controllers;
using PreonSharp.Levenshtein;
using PreonSharp.Loaders;

namespace Preon.WebApi;

internal static class Program
{
    public static void Main(string[] args)
    {
        var app = BuildWebApp(args);

        app.UseHttpLogging();

        var preonController = app.Services.GetRequiredService<PreonController>();

        var preonApi = app.MapGroup("/preon");
        preonApi.MapGet("/", preonController.Query);

        app.Run();
    }
    
    private static WebApplication BuildWebApp(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Logging.AddSimpleConsole(options =>
        {
            options.SingleLine = false;
            options.TimestampFormat = "HH:mm:ss.ffff ";
            options.IncludeScopes = true;
        });

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });

        builder.Services.AddSingleton<PreonController>();

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