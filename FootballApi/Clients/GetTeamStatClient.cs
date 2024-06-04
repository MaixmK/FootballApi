using Newtonsoft.Json;
using Clients;
using API_Football.Models;

namespace API_Football.Clients
{
    public class GetTeamStatClient
    {
        public async Task<TeamStatisticsResponse> GetTeamStatAsync(string ClubName)
        {
            GetLeagueClient Id1 = new GetLeagueClient();
            var Id1result = Id1.GetLeagueAsync(ClubName).Result;
            GetTeamClient Id = new GetTeamClient();
            var Idresult = Id.GetTeamAsync(ClubName).Result;
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api-football-v1.p.rapidapi.com/v3/teams/statistics?league={Id1result.Response[0].league.id}&season=2023&team={Idresult.Response[0].Team.Id}"),
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
                var result = JsonConvert.DeserializeObject<TeamStatisticsResponse>(content);
                return result;
            }
        }
    }
}
