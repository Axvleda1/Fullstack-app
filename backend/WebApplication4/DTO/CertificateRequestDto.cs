using WebApplication4.Models;

namespace WebApplication4.DTOs
{
    public class CertificateRequestDto
    {
        public User User { get; set; }
        public Certificate Certificate { get; set; }
    }
}
