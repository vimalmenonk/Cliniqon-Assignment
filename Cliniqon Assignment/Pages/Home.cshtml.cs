using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Cliniqon_Assignment.Models;

public class HomeModel : PageModel
{
    private readonly IConfiguration _configuration;

    public HomeModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public List<Cliniqon_Assignment.Models.User> Users { get; set; } = new List<Cliniqon_Assignment.Models.User>();

    public async Task OnGetAsync()
    {
        // Fetch connection string from appsettings.json
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // SQL query to fetch user profiles
                string sql = "SELECT Id, Name, ProfilePicture, Designation FROM Users";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Users.Add(new Cliniqon_Assignment.Models.User
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                ProfilePicture = reader["ProfilePicture"].ToString(),
                                Designation = reader["Designation"].ToString()
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (optional logging or error handling)
            Console.WriteLine(ex.Message);
        }
    }
}
