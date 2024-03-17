namespace Normalizer;

public interface IKnowledgeProviderFactory
{
    IEnumerable<IKnowledgeProvider> BuildConfiguredKnowledgeProviders();
}