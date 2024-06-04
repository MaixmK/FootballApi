using Microsoft.AspNetCore.Mvc;
using API_Football.Clients;
using GetLeagueModels;

namespace API_Football.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeagueGet : ControllerBase
    {
        private readonly ILogger<LeagueGet> _logger;
        public LeagueGet(ILogger<LeagueGet> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public GetLeague League(string LeagueName)
        {
            GetLeagueClient client = new GetLeagueClient();
            return client.GetLeagueAsync(LeagueName).Result;
        }
    }
}