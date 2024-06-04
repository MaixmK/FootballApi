using Microsoft.AspNetCore.Mvc;
using API_Football.Clients;

namespace API_Football.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StandingsController : ControllerBase
    {
        private readonly FootballApiClient _footballApiClient;

        public StandingsController(FootballApiClient footballApiClient)
        {
            _footballApiClient = footballApiClient;
        }

        [HttpGet("{ClubName}")]
        public async Task<IActionResult> GetStandings(string ClubName)
        {
            var standings = await _footballApiClient.GetStandingsAsync(ClubName);
            return Ok(standings);
        }
    }
}

