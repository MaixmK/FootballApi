using Microsoft.AspNetCore.Mvc;
using API_Football.Clients;
using GetPlayerModels;

namespace API_Football.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerInfo : ControllerBase
    {
        private readonly ILogger<TeamInfo> _logger;
        public PlayerInfo(ILogger<TeamInfo> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public GetPlayer Player(string ClubName, string PlayerName)
        {
            GetPlayerClient client = new GetPlayerClient();
            return client.GetPlayerAsync(ClubName, PlayerName).Result;
        }
    }
}