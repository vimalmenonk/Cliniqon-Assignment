using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

public class MatchesModel : PageModel
{
    private readonly IConfiguration _configuration;

    public MatchesModel(IConfiguration configuration)
    {
        _configuration = configuration; // Injecting IConfiguration to get connection string
    }

    public List<Match> Matches { get; set; } = new List<Match>();
    public string ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection"); // Fetch the connection string from appsettings.json

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string sql = @"
                    SELECT 
                        Name, 
                        Age, 
                        Gender, 
                        Country, 
                        (CASE 
                            WHEN AgeDifference <= 5 THEN 100 
                            WHEN AgeDifference BETWEEN 6 AND 10 THEN 75 
                            ELSE 50 
                        END) AS MatchPercentage 
                    FROM Users"; // Adjusted matching logic

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Matches.Add(new Match
                            {
                                Name = reader["Name"].ToString(),
                                MatchPercentage = (int)reader["MatchPercentage"],
                                Age = (int)reader["Age"],
                                Gender = reader["Gender"].ToString(),
                                Country = reader["Country"].ToString()
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "An error occurred while fetching matches: " + ex.Message;
        }
    }
}

public class Match
{
    public string Name { get; set; }
    public int MatchPercentage { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string Country { get; set; }
}
