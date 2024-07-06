using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text;
using WebApplication4.Models;
using System.Security.Cryptography;

[Route("api/[controller]")]
[ApiController]
public class RegistrationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public RegistrationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost]
    [Route("registration")]
    public IActionResult Registration([FromBody] User user)
    {

        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("UsersCon")))
        {
            if (UserExists(user.Email))
            {
                return Unauthorized("User Already Exists");
            }

            string selectQuery = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";
            string query = "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)";

            SqlCommand command = new SqlCommand(query, connection);
            SqlCommand selectcommand = new SqlCommand(selectQuery, connection);
            var password = HashPassword(user.Password);

            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", password);

            selectcommand.Parameters.AddWithValue("@Username", user.Username);
            selectcommand.Parameters.AddWithValue("@Email", user.Email);
            selectcommand.Parameters.AddWithValue("@Password", password);
            connection.Open();

            int result = command.ExecuteNonQuery();

            SqlDataReader reader = selectcommand.ExecuteReader();

            if (reader.Read())
            {
                var loggedInUser = new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3)
                };
                return Ok(loggedInUser);
            }
            else
            {
                return BadRequest("Data invalid");
            }
        }
    }

    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromBody] User user)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("UsersCon")))
        {
            string query = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password";
            var password = HashPassword(user.Password);
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", password);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                var loggedInUser = new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3)
                };
                return Ok(loggedInUser);
            }
            else
            {
                return Unauthorized("Invalid credentials");
            }
        }
    }
    private bool UserExists(string email)
    {
        bool exists = false;
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("UsersCon")))
        {
            string query = "SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END FROM Users WHERE Email = @Email";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            connection.Open();
            exists = (int)command.ExecuteScalar() == 1;
            connection.Close();
        }
        return exists;
    }

    public static string HashPassword(string password)
    {

        using var sha = SHA256.Create();

        var asBytes = Encoding.UTF8.GetBytes(password);

        var hashed = sha.ComputeHash(asBytes);

        return Convert.ToBase64String(hashed);

    }
}

