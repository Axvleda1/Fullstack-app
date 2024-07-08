using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

public class CertificateExpiryNotificationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public CertificateExpiryNotificationService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                await CheckAndSendNotifications(emailService);
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task CheckAndSendNotifications(IEmailService emailService)
    {
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("UsersCon")))
        {
            string query = @"
                SELECT u.Email, c.CertificateName, c.ExpiryDate
                FROM Certificates c
                JOIN Users u ON c.UserId = u.UserId
                WHERE DATEDIFF(day, GETDATE(), c.ExpiryDate) IN (60, 30, 7, 1)";

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<CertificateExpiryDto> expiringCertificates = new List<CertificateExpiryDto>();

            while (reader.Read())
            {
                expiringCertificates.Add(new CertificateExpiryDto
                {
                    Email = reader["Email"].ToString(),
                    CertificateName = reader["CertificateName"].ToString(),
                    ExpiryDate = Convert.ToDateTime(reader["ExpiryDate"])
                });
            }

            foreach (var certificate in expiringCertificates)
            {
                string message = $"Your certificate '{certificate.CertificateName}' is expiring on {certificate.ExpiryDate:dd/MM/yyyy}.";
                await emailService.SendEmailAsync(certificate.Email, "Certificate Expiry Notification", message);
            }
        }
    }
}
