using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp;

public interface INormalizerBuilder
{
    IServiceCollection Services { get; }
}
