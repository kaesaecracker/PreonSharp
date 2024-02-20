using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Preon.WebApi.Controllers;

public class PreonController(INormalizer normalizer)
{
    public Task<QueryResult> Query([FromQuery] string s)
    {
        return normalizer.QueryAsync(s);
    }
}