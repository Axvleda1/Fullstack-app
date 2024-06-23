using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebApplication4.DTOs;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CertificatesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("add")]
        public IActionResult AddCertificate([FromBody] CertificateRequestDto dto)
        {
            if (dto.User == null || dto.User.Id == 0)
            {
                return Unauthorized("User not logged in");
            }

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("UsersCon")))
            {
                string query = "INSERT INTO Certificates (UserId, CertificateName, CreationDate, ExpiryDate) VALUES (@UserId, @CertificateName, @CreationDate, @ExpiryDate)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", dto.User.Id);
                command.Parameters.AddWithValue("@CertificateName", dto.Certificate.CertificateName);
                command.Parameters.AddWithValue("@CreationDate", dto.Certificate.CreationDate);
                command.Parameters.AddWithValue("@ExpiryDate", dto.Certificate.ExpiryDate);

                connection.Open();
                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    return Ok("Certificate added");
                }
                else
                {
                    return BadRequest("Failed to add certificate");
                }
            }
        }


        [HttpPost]
        [Route("getCertificates")]
        public IActionResult GetCertificates([FromBody] CertificateRequestDto dto)
        {
            if (dto?.User == null || dto.User.Id == 0)
            {
                return BadRequest("Invalid user information.");
            }

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("UsersCon")))
            {
                string query = "SELECT CertificateName, CreationDate, ExpiryDate FROM Certificates WHERE UserId = @UserId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", dto.User.Id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Certificate> certificates = new List<Certificate>();

                while (reader.Read())
                {
                    certificates.Add(new Certificate
                    {
                        CertificateName = reader["CertificateName"].ToString(),
                        CreationDate = Convert.ToDateTime(reader["CreationDate"]),
                        ExpiryDate = Convert.ToDateTime(reader["ExpiryDate"])
                    });
                }

                return Ok(certificates);
            }
        }
    }

}

