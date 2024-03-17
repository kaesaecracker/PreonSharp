using Microsoft.Extensions.Hosting;

namespace Taxonomy;

public interface IEntitySearcher : IStartAwaitable, IHostedService
{
    Task<TextMatch> GetExactMatches(string text);

    Task<IEnumerable<TextMatch>> GetClosestNames(string text);
}