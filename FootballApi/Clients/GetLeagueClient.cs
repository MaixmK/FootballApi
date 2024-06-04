using Newtonsoft.Json;
using GetLeagueModels;
using Clients;
using API_Football.Models;

namespace API_Football.Clients
{
    public class GetLeagueClient
    {
        public async Task<GetLeague> GetLeagueAsync(string ClubName)
        {
            GetTeamClient Id = new GetTeamClient();
            var Idresult = Id.GetTeamAsync(ClubName).Result;
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api-football-v1.p.rapidapi.com/v3/leagues?team={Idresult.Response[0].Team.Id}"),
                Headers =
                {
                    { "X-RapidAPI-Key", Constants.ApiKey },
                    { "X-RapidAPI-Host", "api-football-v1.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GetLeague>(content);
                return result;
            }
        }
    }
}
