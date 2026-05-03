namespace GtKanu.WebApp.Tests;

using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.Database;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Worker;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly Regex _antiForgeryRegex = new(
        """<input[^>]*name=["]?__RequestVerificationToken["]?[^>]*value=["]?([^"\s>]*)["]?[^>]*>""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly string _certPath;
    private readonly string _keysDir;

    public CustomWebApplicationFactory()
    {
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
        var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".db");

        builder.UseSetting("ConnectionStrings:Sqlite", $"DataSource={tempFile}");
        builder.UseSetting("DataProtection:PfxFile", _certPath);
        builder.UseSetting("DataProtection:PfxPassword", "");
        builder.UseSetting("DataProtection:KeysDirectory", _keysDir);

        builder.ConfigureServices(services =>
        {
            // Remove background worker to prevent concurrent DB access during tests
            var worker = services.Single(d => d.ImplementationType == typeof(HostedWorker));
            services.Remove(worker);

            services.AddAuthorization(options =>
                options.AddPolicy(Policies.TwoFactorAuth, policy => policy.RequireAssertion(_ => true)));
        });
    }

    public string ExtractAntiForgeryToken(string html)
    {
        var match = _antiForgeryRegex.Match(html);
        return match.Success ? match.Groups[1].Value : throw new InvalidOperationException("Anti-forgery token not found");
    }

    public async Task<bool> LoginAsync(HttpClient client, string email, string password)
    {
        var loginPage = await client.GetAsync("/Login");
        var html = await loginPage.Content.ReadAsStringAsync();
        var token = ExtractAntiForgeryToken(html);

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Email"] = email,
            ["Password"] = password,
            ["__RequestVerificationToken"] = token
        });

        var response = await client.PostAsync("/Login", content);
        return response.StatusCode is HttpStatusCode.Redirect or HttpStatusCode.OK;
    }

    public async Task LogoutAsync(HttpClient client)
    {
        await client.GetAsync("/Login/Exit");
    }

    public HttpClient CreateAuthenticatedClient() =>
        CreateDefaultClient(new CookieHandler());

    public async Task EnsureAppStartedAndTestUserCreated(string email, string password, params string[] roles)
    {
        // Ensure server is started and schema is created
        var client = CreateClient();
        await client.GetAsync("/healthz");
        
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUserGuid>>();

        if (await userManager.FindByEmailAsync(email) is null)
        {
            var user = new IdentityUserGuid
            {
                Id = dbContext.GeneratePk(),
                UserName = Guid.NewGuid().ToString(),
                Email = email,
                EmailConfirmed = true,
                Name = "Test User",
                LockoutEnabled = true,
                AccessFailedCount = 0
            };

            var result = await userManager.CreateAsync(user);
            result.Succeeded.Should().BeTrue();
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await userManager.ResetPasswordAsync(user, token, password);
            await userManager.AddToRolesAsync(user, roles);
        }
    }
}
