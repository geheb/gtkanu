namespace GtKanu.Core;

using GtKanu.Core.Database;
using GtKanu.Core.Email;
using GtKanu.Core.Repositories;
using GtKanu.Core.User;
using GtKanu.Core.Worker;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddCore(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IdentityErrorDescriber, GermanyIdentityErrorDescriber>();
        services.AddScoped<AppDbContextInitializer>();

        services.AddScoped<UnitOfWork>();
        services.AddScoped<Foods>();
        services.AddScoped<Bookings>();
        services.AddScoped<Invoices>();
        services.AddScoped<Trips>();
        services.AddScoped<Vehicles>();
        services.AddScoped<Tryouts>();
        services.AddScoped<Boats>();
        services.AddScoped<Clubhouse>();

        services.Configure<SmtpConnectionOptions>(config.GetSection("Smtp"));
        services.AddSingleton<EmailValidatorService>();
        services.AddSingleton<SmtpDispatcher>();
        services.AddScoped<EmailService>();
        services.AddScoped<UserService>();
        services.AddScoped<LoginService>();
        services.AddScoped<IdentityRepository>();

        services.AddHostedService<HostedWorker>();
    }
}
