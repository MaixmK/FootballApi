using API_Football.Models;
using Newtonsoft.Json;

public class SoccerApiClient
{
    private readonly HttpClient _client;

    public SoccerApiClient()
    {
        _client = new HttpClient();
    }

    public async Task<SoccerApiResponse> GetLiveSoccerDataAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://soccer-football-info.p.rapidapi.com/live/basic/?l=en_US&f=json&e=no"),
            Headers =
            {
                { "x-rapidapi-key", Constants.ApiKey },
                { "x-rapidapi-host", "soccer-football-info.p.rapidapi.com" }
            }
        };

        using (var response = await _client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var soccerApiResponse = JsonConvert.DeserializeObject<SoccerApiResponse>(body);
            return soccerApiResponse;
        }
    }
}