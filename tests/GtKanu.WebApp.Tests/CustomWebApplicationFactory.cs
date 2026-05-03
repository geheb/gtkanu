namespace GtKanu.WebApp.Tests;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using GtKanu.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;
    private readonly string _certPath;
    private readonly string _keysDir;

    public CustomWebApplicationFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:;Cache=Shared");
        _connection.Open();

        _certPath = Path.Combine(Path.GetTempPath(), "gtkanu-test.pfx");
        _keysDir = Path.Combine(Path.GetTempPath(), "gtkanu-test-keys");

        if (!File.Exists(_certPath))
        {
            using var rsa = RSA.Create(2048);
            var req = new CertificateRequest("CN=gtkanu-test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var cert = req.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));
            File.WriteAllBytes(_certPath, cert.Export(X509ContentType.Pfx));
        }

        Directory.CreateDirectory(_keysDir);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Sqlite", "DataSource=:memory:");
        builder.UseSetting("DataProtection:PfxFile", _certPath);
        builder.UseSetting("DataProtection:PfxPassword", "");
        builder.UseSetting("DataProtection:KeysDirectory", _keysDir);

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
