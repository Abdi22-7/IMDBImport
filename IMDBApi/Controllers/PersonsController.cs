using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace IMDBApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly string _connStr;

    public PersonsController(IConfiguration config)
    {
        _connStr = config.GetConnectionString("IMDB")!;
    }

    // GET api/persons/search?term=tom
    [HttpGet("search")]
    public async Task<IActionResult> Search(string term)
    {
        var results = new List<object>();
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        using var cmd = new SqlCommand("EXEC sp_SearchPersons @SearchTerm", conn);
        cmd.Parameters.AddWithValue("@SearchTerm", term);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new
            {
                nConst = reader["NConst"],
                primaryName = reader["PrimaryName"],
                birthYear = reader["BirthYear"] == DBNull.Value ? null : (object)reader["BirthYear"],
                deathYear = reader["DeathYear"] == DBNull.Value ? null : (object)reader["DeathYear"]
            });
        }
        return Ok(results);
    }

    // POST api/persons
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddPersonRequest req)
    {
        using var conn = new SqlConnection(_connStr);
        await conn.OpenAsync();
        using var cmd = new SqlCommand("EXEC sp_AddPerson @PrimaryName, @BirthYear", conn);
        cmd.Parameters.AddWithValue("@PrimaryName", req.PrimaryName);
        cmd.Parameters.AddWithValue("@BirthYear", (object?)req.BirthYear ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
        return Ok("Person tilføjet!");
    }
}

public record AddPersonRequest(string PrimaryName, int? BirthYear);