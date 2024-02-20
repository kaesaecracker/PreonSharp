using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace PreonSharp.Loaders;

internal sealed class SepFilesKnowledgeProviderFactory(
    IOptions<SepFilesConfiguration> configuration,
    IServiceProvider serviceProvider)
    : IKnowledgeProviderFactory
{
    private readonly List<SepFileSpec> _specs = configuration.Value.Specs;

    private readonly Func<SepFileSpec, SepFileKnowledgeProvider> _activator =
        s => ActivatorUtilities.CreateInstance<SepFileKnowledgeProvider>(serviceProvider, s);

    public IEnumerable<IKnowledgeProvider> GetKnowledgeProviders() => _specs.Select(_activator);
}