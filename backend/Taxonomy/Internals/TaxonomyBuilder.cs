using Microsoft.Extensions.DependencyInjection;

namespace Taxonomy.Internals;

internal sealed record TaxonomyBuilder(IServiceCollection Services) : ITaxonomyBuilder;