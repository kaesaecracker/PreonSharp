namespace Taxonomy;

public interface IEntityLoader
{
    Task Load(IEntityProviderBuilder builder);
}