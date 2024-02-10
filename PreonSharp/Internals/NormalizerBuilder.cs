using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp.Internals;

internal class NormalizerBuilder(IServiceCollection services) : INormalizerBuilder
{
    public IServiceCollection Services { get; } = services;
}
