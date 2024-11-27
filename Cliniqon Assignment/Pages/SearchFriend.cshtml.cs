using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

public class SearchFriendModel : PageModel
{
    private readonly IConfiguration _configuration;

    public SearchFriendModel(IConfiguration configuration)
    {
        _configuration = configuration; // Injecting IConfiguration to get connection string
    }

    public List<Users> Friends { get; set; } = new List<Users>();
    public string ErrorMessage { get; set; }

    // The method called when AJAX is triggered for search
    public async Task<IActionResult> OnGetSearchAsync(string name)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection"); // Fetch the connection string

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                string sql = "SELECT * FROM Users WHERE Name LIKE @Name";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", "%" + name + "%");
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Friends.Add(new Users
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                ProfilePicture = reader["ProfilePicture"].ToString()
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "An error occurred while searching for friends: " + ex.Message;
        }

        return new JsonResult(Friends); // Return the list of matching friends as JSON for AJAX
    }
}

public class Users
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ProfilePicture { get; set; }
}
