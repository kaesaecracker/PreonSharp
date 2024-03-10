namespace PreonSharp;

public interface IKnowledgeProviderFactory
{
    IEnumerable<IKnowledgeProvider> BuildConfiguredKnowledgeProviders();
}