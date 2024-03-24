using Microsoft.Extensions.DependencyInjection;
using Taxonomy;

namespace Loaders.Ncbi;

public static class NcbiExtensions
{
    public static void AddNcbiEntityLoader(this ITaxonomyBuilder builder)
    {
        builder.Services.AddSingleton<IEntityLoader, NcbiTaxonomyEntityLoader>();
        builder.Services.AddSingleton<IEntityLoader, NcbiImageLoader>();
        builder.Services.AddSingleton<IEntityLoader, NcbiGeneInfoEntityLoader>();
    }
}