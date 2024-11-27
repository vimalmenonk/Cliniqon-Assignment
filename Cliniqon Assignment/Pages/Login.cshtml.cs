using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;

public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;

    // Inject IConfiguration into the constructor
    public LoginModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Bind the form inputs
    [BindProperty]
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }

    // Add the LoginFailed property to indicate login failure
    public bool LoginFailed { get; set; }  // Ensure this property is present

    public void OnGet()
    {
        // Initialize the page for GET requests
        LoginFailed = false;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Check for valid model state
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();

            string sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email AND Password = @Password";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Password", Password);

                var result = (int)await cmd.ExecuteScalarAsync();

                if (result > 0)
                {
                    
                    return RedirectToPage("/Home"); 
                }
                else
                {
                    LoginFailed = true;
                    return Page();
                }
            }
        }
    }
}
