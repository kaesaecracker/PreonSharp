using Microsoft.Extensions.Hosting;

namespace Taxonomy;

public interface IEntityProvider : IHostedService, IStartAwaitable
{
    Task<Entity?> GetById(Guid id);
    
    Task<IEnumerable<Entity>> GetFirst(int count);
    
    IEnumerable<Entity> All { get; }
}