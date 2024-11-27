using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;  // Import this namespace
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

public class SignupModel : PageModel
{
    private readonly IConfiguration _configuration;

    // Inject IConfiguration into the constructor
    public SignupModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, ErrorMessage = "Password should be at least 6 characters long.", MinimumLength = 6)]
    public string Password { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Date of Birth is required.")]
    public string DOB { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Designation is required.")]
    public string Designation { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Gender is required.")]
    public string Gender { get; set; }

    [BindProperty]
    public string ProfilePicture { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Country is required.")]
    public string Country { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Favorite Color is required.")]
    public string FavoriteColor { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Favorite Actor is required.")]
    public string FavoriteActor { get; set; }

    public async Task OnPostAsync()
    {
        if (!ModelState.IsValid)
            return;

        // Get connection string from appsettings.json
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            try
            {
                // Open the connection
                await conn.OpenAsync();

                // SQL query
                string sql = "INSERT INTO Users (Name, Email, Password, DOB, Designation, Gender, ProfilePicture, Country, FavoriteColor, FavoriteActor) " +
                             "VALUES (@Name, @Email, @Password, @DOB, @Designation, @Gender, @ProfilePicture, @Country, @FavoriteColor, @FavoriteActor)";

                // Prepare the command with parameters
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Password", Password); // Ensure to hash the password before inserting
                    cmd.Parameters.AddWithValue("@DOB", DateTime.Parse(DOB)); // Ensure DOB is in the correct format
                    cmd.Parameters.AddWithValue("@Designation", Designation);
                    cmd.Parameters.AddWithValue("@Gender", Gender);
                    cmd.Parameters.AddWithValue("@ProfilePicture", string.IsNullOrEmpty(ProfilePicture) ? DBNull.Value : (object)ProfilePicture);
                    cmd.Parameters.AddWithValue("@Country", Country);
                    cmd.Parameters.AddWithValue("@FavoriteColor", FavoriteColor);
                    cmd.Parameters.AddWithValue("@FavoriteActor", FavoriteActor);

                    // Execute the query
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Record inserted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Insert failed.");
                    }
                }
            }
            catch (SqlException ex)
            {
                // Handle SQL errors
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle any other errors
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
