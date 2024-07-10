var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<CertificateExpiryNotificationService>();
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddRazorPages();

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


app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});
app.MapControllers();
app.UseStaticFiles();

app.Run();
