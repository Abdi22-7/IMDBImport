using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace IMDBApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TitlesController : ControllerBase
{
    private readonly string _connStr;

    public TitlesController(IConfiguration config)
    {
        _connStr = config.GetConnectionString("IMDB")!;
    }

    // GET api/titles/search?term=batman
    [HttpGet("search")]
    public async Task<IActionResult> Search(string term)
    {
        var results = new List<object>();
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        using var cmd = new SqlCommand("EXEC sp_SearchTitles @SearchTerm", conn);
        cmd.CommandTimeout = 120;
        cmd.Parameters.AddWithValue("@SearchTerm", term);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new
            {
                tConst = reader["TConst"],
                titleType = reader["TitleType"],
                primaryTitle = reader["PrimaryTitle"],
                startYear = reader["StartYear"] == DBNull.Value ? null : (object)reader["StartYear"]
            });
        }
        return Ok(results);
    }

    // POST api/titles
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddTitleRequest req)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        using var cmd = new SqlCommand("EXEC sp_AddTitle @TitleType, @PrimaryTitle, @OriginalTitle, @StartYear, @IsAdult", conn);
        cmd.CommandTimeout = 120;
        cmd.Parameters.AddWithValue("@TitleType", req.TitleType);
        cmd.Parameters.AddWithValue("@PrimaryTitle", req.PrimaryTitle);
        cmd.Parameters.AddWithValue("@OriginalTitle", req.OriginalTitle);
        cmd.Parameters.AddWithValue("@StartYear", (object?)req.StartYear ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@IsAdult", req.IsAdult);
        await cmd.ExecuteNonQueryAsync();
        return Ok("Film tilføjet!");
    }

    // PUT api/titles/5
    [HttpPut("{tconst}")]
    public async Task<IActionResult> Update(int tconst, [FromBody] UpdateTitleRequest req)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        using var cmd = new SqlCommand("EXEC sp_UpdateTitle @TConst, @PrimaryTitle, @StartYear", conn);
        cmd.CommandTimeout = 120;
        cmd.Parameters.AddWithValue("@TConst", tconst);
        cmd.Parameters.AddWithValue("@PrimaryTitle", req.PrimaryTitle);
        cmd.Parameters.AddWithValue("@StartYear", (object?)req.StartYear ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
        return Ok("Film opdateret!");
    }

    // DELETE api/titles/5
    [HttpDelete("{tconst}")]
    public async Task<IActionResult> Delete(int tconst)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        using var cmd = new SqlCommand("EXEC sp_DeleteTitle @TConst", conn);
        cmd.CommandTimeout = 120;
        cmd.Parameters.AddWithValue("@TConst", tconst);
        await cmd.ExecuteNonQueryAsync();
        return Ok("Film slettet!");
    }
}

public record AddTitleRequest(string TitleType, string PrimaryTitle, string OriginalTitle, int? StartYear, bool IsAdult);
public record UpdateTitleRequest(string PrimaryTitle, int? StartYear);