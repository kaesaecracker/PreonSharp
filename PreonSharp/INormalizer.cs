using Microsoft.Extensions.Hosting;

namespace PreonSharp;

public interface INormalizer: IHostedService
{
    Task WaitForInitializationAsync();
    
    Task<QueryResult> QueryAsync(string queryName, CancellationToken? token = null);
}
