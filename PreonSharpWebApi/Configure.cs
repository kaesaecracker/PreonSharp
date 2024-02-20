using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Preon.WebApi.Controllers;
using PreonSharp.Levenshtein;
using PreonSharp.Loaders;

namespace Preon.WebApi;

public static class Configure
{
    public static WebApplication BuildWebApp(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

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