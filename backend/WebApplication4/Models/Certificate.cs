namespace WebApplication4.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CertificateName { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

}
