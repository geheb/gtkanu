using GtKanu.Core;
using GtKanu.Core.Database;
using GtKanu.Core.Email;
using GtKanu.Core.Entities;
using GtKanu.Core.User;
using GtKanu.WebApp.Annotations;
using GtKanu.WebApp.Bindings;
using GtKanu.WebApp.Constants;
using GtKanu.WebApp.Filters;
using GtKanu.WebApp.Middlewares;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Serilog;

void ConfigureApp(WebApplicationBuilder builder)
{
    builder.WebHost.ConfigureKestrel(o =>
    {
        o.AddServerHeader = false;
        o.AllowResponseHeaderCompression = false;
    });

    var config = builder.Configuration;
    var services = builder.Services;

    services.AddMySqlContext(config);
    services.AddMemoryCache();

    services
       .AddIdentity<IdentityUserGuid, IdentityRoleGuid>(options =>
       {
           options.SignIn.RequireConfirmedEmail = true;
           options.Tokens.EmailConfirmationTokenProvider = ConfirmEmailDataProtectionTokenProviderOptions.ProviderName;
       })
       .AddEntityFrameworkStores<AppDbContext>()
       .AddDefaultTokenProviders()
       .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider<IdentityUserGuid>>(ConfirmEmailDataProtectionTokenProviderOptions.ProviderName);       
       
    services.AddAuthorizationBuilder()
        .AddPolicy(Policies.TwoFactorAuth, policy => policy.RequireClaim(UserClaims.TwoFactorClaim.Type, UserClaims.TwoFactorClaim.Value));

    services.AddAuthorization();

    services.AddHttpContextAccessor();

    services.AddRazorPages()
        .AddMvcOptions(options =>
        {
            options.ModelBindingMessageProvider.SetLocale();
            options.Filters.Add<OperationCancelledExceptionFilter>();
        });

    services.AddDataProtection()
        .AddCertificate(config.GetSection("DataProtection"));

    services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = CookieNames.AppToken;
        options.ExpireTimeSpan = TimeSpan.FromHours(1);

        options.LoginPath = "/Login";
        options.LogoutPath = "/Login/Exit";
        options.AccessDeniedPath = "/Error/403";
        options.SlidingExpiration = true;
    });

    services.Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorRememberMeScheme, options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = CookieNames.TwoFactorTrustToken;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

    services.Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorUserIdScheme, options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = CookieNames.TwoFactorIdToken;
    });

    services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = PasswordLengthFieldAttribute.MinLen;
        options.Password.RequiredUniqueChars = 5;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(90);
        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.AllowedForNewUsers = true;

        options.User.RequireUniqueEmail = true;
    });

    services.Configure<DataProtectionTokenProviderOptions>(options =>
    {
        // lifespan of issued tokens (changing email or password)
        options.TokenLifespan = TimeSpan.FromDays(5);
    });

    services.Configure<ConfirmEmailDataProtectionTokenProviderOptions>(options =>
    {
        // lifespan of issued tokens (confirm email)
        options.TokenLifespan = TimeSpan.FromDays(5);
    });

    services.Configure<SecurityStampValidatorOptions>(options =>
    {
        // A user accessing the site with an existing cookie would be validated, and a new cookie would be issued. 
        // This process is completely silent and happens behind the scenes.
        options.ValidationInterval = TimeSpan.FromMinutes(30);
    });

    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });

    services.Configure<AntiforgeryOptions>(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Name = CookieNames.XcsrfToken;
    });

    services.Configure<AppSettings>(config.GetSection("App"));
    services.AddSingleton<NodeGeneratorService>();
    services.AddCore(config);
}

void ConfigurePipeline(WebApplication app)
{
    app.UseForwardedHeaders();

    app.UseSerilogRequestLogging(o =>
    {
        // Customize the message template
        o.MessageTemplate = "{RemoteIpAddress} @ {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        // Attach additional properties to the request completion event
        o.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
        };
    });

    app.UseRequestLocalization("de-DE");

    // Configure the HTTP request pipeline.
    app.UseExceptionHandler("/Error/500");

    app.UseMiddleware<CspMiddleware>();

    app.UseStatusCodePagesWithReExecute("/Error/{0}");

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseNodeGenerator();
}

try
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("logging.json")
        .Build();

    var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

    Log.Logger = logger;

    Log.Information("Application started");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    ConfigureApp(builder);
    using var app = builder.Build();
    ConfigurePipeline(app);
    app.MapRazorPages();
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.Information("Application exited");
    await Log.CloseAndFlushAsync();
}
