using API_Football.Clients;
using Microsoft.AspNetCore.Mvc;

namespace API_Football.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopScorersController : ControllerBase
    {
        private readonly ILogger<TopScorersController> _logger;
        private readonly TopScorersClient _topScorersClient;

        public TopScorersController(ILogger<TopScorersController> logger, TopScorersClient topScorersClient)
        {
            _logger = logger;
            _topScorersClient = topScorersClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetTopScorers(string TeamName)
        {
            var topScorers = await _topScorersClient.GetTopScorersAsync(TeamName);
            return Ok(topScorers);
        }
    }
}

