using API_Football.Models;
using Newtonsoft.Json;

namespace API_Football.Clients
{
    public class TopScorersClient
    {
        private readonly HttpClient _httpClient;

        public TopScorersClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TopScorersResponse> GetTopScorersAsync(string ClubName)
        {
            GetLeagueClient Id1 = new GetLeagueClient();
            var Id1result = await Id1.GetLeagueAsync(ClubName);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api-football-v1.p.rapidapi.com/v3/players/topscorers?league={Id1result.Response[0].league.id}&season=2023"),
                Headers =
                {
                    { "X-RapidAPI-Key", Constants.ApiKey },
                    { "X-RapidAPI-Host", "api-football-v1.p.rapidapi.com" },
                },
            };

            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonConvert.DeserializeObject<dynamic>(body);
                var topScorersResponse = new TopScorersResponse();

                foreach (var item in apiResponse.response)
                {
                    var player = new Player
                    {
                        Id = item.player.id,
                        Name = item.player.name,
                        Nationality = item.player.nationality,
                        Photo = item.player.photo,
                        Age = item.player.age,
                        Goals = item.statistics[0].goals.total,
                        Assists = item.statistics[0].goals.assists,
                        Team = item.statistics[0].team.name
                    };

                    topScorersResponse.Players.Add(player);
                }

                return topScorersResponse;
            }
        }
    }
}

