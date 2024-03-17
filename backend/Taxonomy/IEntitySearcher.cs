using Microsoft.Extensions.Hosting;

namespace Taxonomy;

public interface IEntitySearcher : IStartAwaitable, IHostedService
{
    Task<IEnumerable<Entity>> GetExactMatches(string text);
}