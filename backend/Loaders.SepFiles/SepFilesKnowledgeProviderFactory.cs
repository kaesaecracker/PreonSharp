using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Normalizer;

namespace Loaders.SepFiles;

internal sealed class SepFilesKnowledgeProviderFactory(
    IOptions<SepFilesConfiguration> configuration,
    IServiceProvider serviceProvider)
    : IKnowledgeProviderFactory
{
    private readonly List<SepFileSpec> _specs = configuration.Value.Specs;

    private readonly Func<SepFileSpec, SepFileKnowledgeProvider> _activator =
        s => ActivatorUtilities.CreateInstance<SepFileKnowledgeProvider>(serviceProvider, s);

    public IEnumerable<IKnowledgeProvider> BuildConfiguredKnowledgeProviders() => _specs.Select(_activator);
}