using Microsoft.Extensions.Hosting;

namespace Taxonomy;

public interface IEntityProvider : IHostedService
{
    Task<Entity?> GetById(Guid id);
    Task<IEnumerable<Entity>> GetFirst(int count);
}