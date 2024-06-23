using LogInBack.Models;
using System.Net.Mail;
using System.Net;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpConfig = _configuration.GetSection("Smtp").Get<SmtpConfiguration>();

        using (var client = new SmtpClient(smtpConfig.Host, smtpConfig.Port))
        {
            client.Credentials = new NetworkCredential(smtpConfig.Username, smtpConfig.Password);
            client.EnableSsl = smtpConfig.Ssl;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpConfig.From),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}
