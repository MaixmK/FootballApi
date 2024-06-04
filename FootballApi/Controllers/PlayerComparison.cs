using Microsoft.AspNetCore.Mvc;
using Npgsql;
using PostPlayer;
using API_Football.Clients;
using GetPlayerModels;
using API_Football.Models;

namespace Football_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerComparisonController : ControllerBase
    {
        private readonly ILogger<PlayerComparisonController> _logger;
        public PlayerComparisonController(ILogger<PlayerComparisonController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ComparePlayer/{FootballClub}/{PlayerName}")]
        public async Task<IActionResult> ComparePlayer(string FootballClub, string PlayerName)
        {
            try
            {
                string Connect = $"Host=localhost;Username=postgres;Password={Constants.BDPassword};Database=postgres";
                using var con = new NpgsqlConnection(Connect);
                var client = new GetPlayerClient();
                var playerResponse = (await client.GetPlayerAsync(FootballClub, PlayerName)).Response[0];

                var player = new Players
                {
                    Id = playerResponse.Player.Id,
                    Firstname = playerResponse.Player.Firstname,
                    Lastname = playerResponse.Player.Lastname,
                    Age = playerResponse.Player.Age,
                    Height = playerResponse.Player.Height,
                    Weight = playerResponse.Player.Weight,
                    Nationality = playerResponse.Player.Nationality,
                    Clubname = playerResponse.Statistics[0].Team.Name,
                    Position = playerResponse.Statistics[0].Games.Position,
                    Rating = playerResponse.Statistics[0].Games.Rating,
                    Total = playerResponse.Statistics[0].Goals.Total,
                    Assists = playerResponse.Statistics[0].Goals.Assists
                };

                var sql = "SELECT \"id\", \"firstname\", \"lastname\", \"age\", \"height\", \"weight\", \"nationality\", \"clubname\", \"position\", \"total\", \"assists\" " +
                          "FROM public.\"Player\" WHERE \"position\" = @position";
                using var comm = new NpgsqlCommand(sql, con);
                comm.Parameters.AddWithValue("position", player.Position);

                await con.OpenAsync();
                using var reader = await comm.ExecuteReaderAsync();

                var players = new List<Players>();
                while (await reader.ReadAsync())
                {
                    players.Add(new Players
                    {
                        Id = reader.GetInt32(0),
                        Firstname = reader.GetString(1),
                        Lastname = reader.GetString(2),
                        Age = reader.GetInt32(3),
                        Height = reader.GetString(4),
                        Weight = reader.GetString(5),
                        Nationality = reader.GetString(6),
                        Clubname = reader.GetString(7),
                        Position = reader.GetString(8),
                        Total = reader.IsDBNull(9) ? (int?)null : reader.GetInt32(9),
                        Assists = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10)
                    });
                }
                await con.CloseAsync();

                var comparisons = players.Select(p => new
                {
                    p.Firstname,
                    p.Lastname,
                    p.Position,
                    p.Total,
                    p.Assists,
                    GoalsComparison = CompareGoals(player, p),
                    AssistsComparison = CompareAssists(player, p)
                }).ToList();

                _logger.LogInformation("Player comparison completed successfully.");
                return Ok(comparisons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing player.");
                return StatusCode(500, "Internal server error");
            }
        }

        private string CompareGoals(Players player1, Players player2)
        {
            if (!player1.Total.HasValue || !player2.Total.HasValue)
            {
                return "N/A";
            }

            return $"{player1.Total} vs {player2.Total} - {(player1.Total > player2.Total ? $"{player1.Firstname} {player1.Lastname} has more goals" : player1.Total < player2.Total ? $"{player2.Firstname} {player2.Lastname} has more goals" : "Equal goals")}";
        }

        private string CompareAssists(Players player1, Players player2)
        {
            if (!player1.Assists.HasValue || !player2.Assists.HasValue)
            {
                return "N/A";
            }

            return $"{player1.Assists} vs {player2.Assists} - {(player1.Assists > player2.Assists ? $"{player1.Firstname} {player1.Lastname} has more assists" : player1.Assists < player2.Assists ? $"{player2.Firstname} {player2.Lastname} has more assists" : "Equal assists")}";
        }
    }
}
