using Microsoft.Extensions.DependencyInjection;

namespace Taxonomy;

public interface ITaxonomyBuilder
{
    IServiceCollection Services { get; }
}

internal sealed record TaxonomyBuilder(IServiceCollection Services) : ITaxonomyBuilder;