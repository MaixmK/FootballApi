using Microsoft.AspNetCore.Mvc;
using API_Football.Clients;
using GetTeamModels;
using Clients;

namespace API_Football.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeamInfo : ControllerBase
    {
        private readonly ILogger<TeamInfo> _logger;
        public TeamInfo(ILogger<TeamInfo> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public GetTeam Team(string TeamName)
        {
            GetTeamClient client = new GetTeamClient();
            return client.GetTeamAsync(TeamName).Result;
        }
    }
    [ApiController]
    [Route("[controller]")]
    public class TeamStatController : ControllerBase
    {
        private readonly ILogger<TeamStatController> _logger;
        public TeamStatController(ILogger<TeamStatController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeamStatAsync(string teamName)
        {
                GetTeamStatClient client = new GetTeamStatClient();
                var result = await client.GetTeamStatAsync(teamName);
                return Ok(result);
        }
    }
}