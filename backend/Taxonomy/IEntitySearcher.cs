using Microsoft.Extensions.Hosting;

namespace Taxonomy;

public interface IEntitySearcher : IStartAwaitable, IHostedService
{
    Task<TextMatch> GetExactMatches(string text);

    Task<ISet<TextMatch>> GetClosestNames(string text);
}