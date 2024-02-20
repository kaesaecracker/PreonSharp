using System.Threading.Tasks;

namespace Preon.WebApi.Controllers;

public class PreonController(INormalizer normalizer)
{
    public Task<QueryResult> Query(string text)
    {
        return normalizer.QueryAsync(text);
    }
}