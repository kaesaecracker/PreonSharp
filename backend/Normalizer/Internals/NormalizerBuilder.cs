namespace Normalizer.Internals;

internal sealed class NormalizerBuilder(IServiceCollection services) : INormalizerBuilder
{
    public IServiceCollection Services { get; } = services;
}
