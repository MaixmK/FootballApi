using Newtonsoft.Json;
using GetPlayerModels;
using Clients;
using API_Football.Models;

namespace API_Football.Clients
{
    public class GetPlayerClient
    {
        public async Task<GetPlayer> GetPlayerAsync(string ClubName, string PlayersName)
        {
            GetTeamClient Id = new GetTeamClient();
            var Idresult = Id.GetTeamAsync(ClubName).Result;
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api-football-v1.p.rapidapi.com/v3/players?team={Idresult.Response[0].Team.Id}&search={PlayersName}&season=2023"),
                Headers =
            {
                { "X-RapidAPI-Key", Constants.ApiKey },
                { "X-RapidAPI-Host", "api-football-v1.p.rapidapi.com" }
            }
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GetPlayer>(content);
                return result;
            }
        }
    }
}
