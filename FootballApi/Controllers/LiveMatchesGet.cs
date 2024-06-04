using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SoccerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SoccerController : ControllerBase
    {
        private readonly SoccerApiClient _soccerApiClient;

        public SoccerController()
        {
            _soccerApiClient = new SoccerApiClient();
        }

        [HttpGet("live")]
        public async Task<IActionResult> GetLiveSoccerData()
        {
                var soccerData = await _soccerApiClient.GetLiveSoccerDataAsync();
                return Ok(soccerData);  
        }
    }
}