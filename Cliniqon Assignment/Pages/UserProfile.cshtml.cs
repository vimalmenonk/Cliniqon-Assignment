using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Cliniqon_Assignment.Models;

public class UserProfileModel : PageModel
{
    private readonly IConfiguration _configuration;

    public UserProfileModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Define a User property to hold the fetched user data
    public User User { get; set; }

    public async Task OnGetAsync(int id)
    {
        // Fetch connection string from appsettings.json
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // SQL query to fetch user details by ID
                string sql = "SELECT Id, Name, ProfilePicture, Designation, Email, Bio FROM Users WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            User = new User
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                ProfilePicture = reader["ProfilePicture"].ToString(),
                                Designation = reader["Designation"].ToString(),
                                Email = reader["Email"].ToString(),
                                Bio = reader["Bio"].ToString()
                            };
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
