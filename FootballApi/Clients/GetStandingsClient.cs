using API_Football.Models;
using GetStandingsModels;
using Newtonsoft.Json;

namespace API_Football.Clients
{
    public class FootballApiClient
    {
        private readonly HttpClient _client;

        public FootballApiClient(HttpClient client)
        {
            _client = client;
            _client.DefaultRequestHeaders.Add("x-rapidapi-key", Constants.ApiKey);
            _client.DefaultRequestHeaders.Add("x-rapidapi-host", "api-football-v1.p.rapidapi.com");
        }

        public async Task<ApiResponse> GetStandingsAsync(string ClubName)
        {
            GetLeagueClient Id1 = new GetLeagueClient();
            var Id1result = Id1.GetLeagueAsync(ClubName).Result;
            var requestUri = new Uri($"https://api-football-v1.p.rapidapi.com/v3/standings?league={Id1result.Response[0].league.id}&season=2023");
            var response = await _client.GetAsync(requestUri);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ApiResponse>(content);
        }
    }

}
