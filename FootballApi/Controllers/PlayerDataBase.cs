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
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;
        public PlayerController(ILogger<PlayerController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PlayerPost")]
        public async Task<IActionResult> Player(string FootballClub, string PlayerName)
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

                var sql = "INSERT INTO public.\"Player\"(\"id\", \"firstname\", \"lastname\", \"age\", \"height\", \"weight\", \"nationality\", \"clubname\", \"position\", \"rating\", \"total\", \"assists\")" +
                          " VALUES (@id, @firstname, @lastname, @age, @height, @weight, @nationality, @clubname, @position, @rating, @total, @assists)";
                using var comm = new NpgsqlCommand(sql, con);
                comm.Parameters.AddWithValue("id", player.Id);
                comm.Parameters.AddWithValue("firstname", player.Firstname);
                comm.Parameters.AddWithValue("lastname", player.Lastname);
                comm.Parameters.AddWithValue("age", player.Age);
                comm.Parameters.AddWithValue("height", player.Height);
                comm.Parameters.AddWithValue("weight", player.Weight);
                comm.Parameters.AddWithValue("nationality", player.Nationality);
                comm.Parameters.AddWithValue("clubname", player.Clubname);
                comm.Parameters.AddWithValue("position", player.Position);
                comm.Parameters.AddWithValue("rating", player.Rating);
                comm.Parameters.AddWithValue("total", player.Total ?? (object)DBNull.Value);
                comm.Parameters.AddWithValue("assists", player.Assists ?? (object)DBNull.Value);

                await con.OpenAsync();
                await comm.ExecuteNonQueryAsync();
                await con.CloseAsync();

                _logger.LogInformation("Player added successfully.");
                return Ok(new { message = "Player added successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding player.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{currentLastname}/{newPlayerClub}/{newPlayerName}")]
        public async Task<IActionResult> UpdatePlayer(string currentLastname, string newPlayerClub, string newPlayerName)
        {
            try
            {
                string Connect = $"Host=localhost;Username=postgres;Password={Constants.BDPassword};Database=postgres";
                using var con = new NpgsqlConnection(Connect);
                var client = new GetPlayerClient();
                var getPlayerResponse = await client.GetPlayerAsync(newPlayerClub, newPlayerName);
                var newPlayerResponse = getPlayerResponse.Response.FirstOrDefault();

                if (newPlayerResponse == null)
                {
                    _logger.LogWarning($"Player not found: {newPlayerName} in club: {newPlayerClub}");
                    return NotFound(new { message = "Player not found." });
                }

                var newPlayer = newPlayerResponse;

                var sql = @"UPDATE public.""Player"" 
                    SET ""id"" = @id, 
                        ""firstname"" = @firstname, 
                        ""lastname"" = @newLastname, 
                        ""age"" = @age, 
                        ""height"" = @height, 
                        ""weight"" = @weight, 
                        ""nationality"" = @nationality, 
                        ""clubname"" = @clubname, 
                        ""position"" = @position, 
                        ""rating"" = @rating, 
                        ""total"" = @total, 
                        ""assists"" = @assists 
                    WHERE ""lastname"" = @currentLastname";

                using var comm = new NpgsqlCommand(sql, con);
                comm.Parameters.AddWithValue("id", newPlayer.Player.Id);
                comm.Parameters.AddWithValue("firstname", newPlayer.Player.Firstname);
                comm.Parameters.AddWithValue("newLastname", newPlayer.Player.Lastname);
                comm.Parameters.AddWithValue("age", newPlayer.Player.Age);
                comm.Parameters.AddWithValue("height", newPlayer.Player.Height);
                comm.Parameters.AddWithValue("weight", newPlayer.Player.Weight);
                comm.Parameters.AddWithValue("nationality", newPlayer.Player.Nationality);
                comm.Parameters.AddWithValue("position", newPlayer.Statistics.FirstOrDefault()?.Games.Position ?? "");
                comm.Parameters.AddWithValue("rating", newPlayer.Statistics.FirstOrDefault()?.Games.Rating ?? "");
                comm.Parameters.AddWithValue("clubname", newPlayer.Statistics.FirstOrDefault()?.Team.Name ?? "");
                comm.Parameters.AddWithValue("total", newPlayer.Statistics.FirstOrDefault()?.Goals.Total ?? (object)DBNull.Value);
                comm.Parameters.AddWithValue("assists", newPlayer.Statistics.FirstOrDefault()?.Goals.Assists ?? (object)DBNull.Value);
                comm.Parameters.AddWithValue("currentLastname", currentLastname);

                await con.OpenAsync();
                int rowsAffected = await comm.ExecuteNonQueryAsync();
                await con.CloseAsync();

                if (rowsAffected > 0)
                {
                    _logger.LogInformation($"Player updated successfully: {newPlayer.Player.Lastname}");
                    return Ok(new { message = "Player updated successfully." });
                }
                else
                {
                    _logger.LogWarning($"Player not found: {currentLastname}");
                    return NotFound(new { message = "Player not found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating player.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{lastname}")]
        public async Task<IActionResult> DeletePlayer(string lastname)
        {
            try
            {
                string Connect = $"Host=localhost;Username=postgres;Password={Constants.BDPassword};Database=postgres";
                using var con = new NpgsqlConnection(Connect);

                var sql = "DELETE FROM public.\"Player\" WHERE \"lastname\" = @lastname";
                using var comm = new NpgsqlCommand(sql, con);
                comm.Parameters.AddWithValue("lastname", lastname);

                await con.OpenAsync();
                int rowsAffected = await comm.ExecuteNonQueryAsync();
                await con.CloseAsync();

                if (rowsAffected > 0)
                {
                    _logger.LogInformation($"Player deleted successfully: {lastname}");
                    return Ok(new { message = "Player deleted successfully." });
                }
                else
                {
                    _logger.LogWarning($"Player not found: {lastname}");
                    return NotFound(new { message = "Player not found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting player.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("ClearDatabase")]
        public async Task<IActionResult> ClearDatabase()
        {
            try
            {
                string Connect = $"Host=localhost;Username=postgres;Password={Constants.BDPassword};Database=postgres";
                using var con = new NpgsqlConnection(Connect);

                var playerSql = "DELETE FROM public.\"Player\"";
                using var playerComm = new NpgsqlCommand(playerSql, con);

                await con.OpenAsync();
                int rowsAffected = await playerComm.ExecuteNonQueryAsync();

                await con.CloseAsync();

                _logger.LogInformation($"All players deleted successfully: {rowsAffected}");
                return Ok(new { message = "All players deleted successfully.", rowsAffected });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing database.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("ListPlayers")]
        public async Task<IActionResult> ListPlayers()
        {
            try
            {
                string Connect = $"Host=localhost;Username=postgres;Password={Constants.BDPassword};Database=postgres";
                using var con = new NpgsqlConnection(Connect);

                var sql = "SELECT \"id\", \"firstname\", \"lastname\", \"age\", \"height\", \"weight\", \"nationality\", \"clubname\", \"position\", \"rating\", \"total\", \"assists\" FROM public.\"Player\"";
                using var comm = new NpgsqlCommand(sql, con);

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
                        Rating = reader.GetString(9),
                        Total = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                        Assists = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11)
                    });
                }

                await con.CloseAsync();

                _logger.LogInformation("Players retrieved successfully.");
                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving players.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
