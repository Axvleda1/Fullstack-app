var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<CertificateExpiryNotificationService>();
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:44310")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
