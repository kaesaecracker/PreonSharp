using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Preon.WebApi.Controllers;

namespace Preon.WebApi;

internal static class Program
{
    public static void Main(string[] args)
    {
        var app = Configure.BuildWebApp(args);

        var preonController = app.Services.GetRequiredService<PreonController>();

        var preonApi = app.MapGroup("/preon");
        preonApi.MapGet("/", preonController.Query);

        app.Run();
    }
}