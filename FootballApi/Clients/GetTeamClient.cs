using API_Football.Models;
using GetTeamModels;
using Newtonsoft.Json;

namespace Clients;

public class GetTeamClient
{
    public async Task<GetTeam> GetTeamAsync(string TeamName)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://api-football-v1.p.rapidapi.com/v3/teams?name={TeamName}"),
            Headers = {
                { "X-RapidAPI-Key", Constants.ApiKey },
                { "X-RapidAPI-Host", "api-football-v1.p.rapidapi.com" },
            },
        };
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<GetTeam>(content);
            return result;
        }
    }
}
