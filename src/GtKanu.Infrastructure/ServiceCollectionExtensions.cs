namespace GtKanu.Infrastructure;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Application.Services;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.Database;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Database.Repositories;
using GtKanu.Infrastructure.Email;
using GtKanu.Infrastructure.User;
using GtKanu.Infrastructure.Worker;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class ServiceCollectionExtensions
{
    public static void AddMySqlContext(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblyName = typeof(AppDbContext).GetTypeInfo().Assembly.GetName().Name;

        services.AddDbContext<AppDbContext>(options =>
        {
            options.ConfigureWarnings(warn => warn.Ignore(
                CoreEventId.FirstWithoutOrderByAndFilterWarning,
                CoreEventId.RowLimitingOperationWithoutOrderByWarning,
                CoreEventId.DistinctAfterOrderByWithoutRowLimitingOperatorWarning));

            options.UseMySql(
                configuration.GetConnectionString("MySql"),
                MariaDbServerVersion.LatestSupportedServerVersion,
                mysqlOptions =>
                {
                    mysqlOptions.MaxBatchSize(100);
                    mysqlOptions.MigrationsAssembly(assemblyName);
                });
        });
    }

    public static void AddIdentityStore(this IServiceCollection services)
    {
        services
           .AddIdentity<IdentityUserGuid, IdentityRoleGuid>(options =>
           {
               options.SignIn.RequireConfirmedEmail = true;
               options.Tokens.EmailConfirmationTokenProvider = ConfirmEmailDataProtectionTokenProviderOptions.ProviderName;
           })
           .AddEntityFrameworkStores<AppDbContext>()
           .AddDefaultTokenProviders()
           .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider<IdentityUserGuid>>(ConfirmEmailDataProtectionTokenProviderOptions.ProviderName);
    }

    public static void AddCore(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<AppSettings>(config.GetSection("App"));
        services.AddSingleton<NodeGeneratorService>();

        services.AddScoped<IdentityErrorDescriber, GermanyIdentityErrorDescriber>();
        services.AddScoped<AppDbContextInitializer>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IFoods, Foods>();
        services.AddScoped<IBookings, Bookings>();
        services.AddScoped<IInvoices, Invoices>();
        services.AddScoped<ITrips, Trips>();
        services.AddScoped<IVehicles, Vehicles>();
        services.AddScoped<ITryouts, Tryouts>();
        services.AddScoped<IBoats, Boats>();
        services.AddScoped<IClubhouse, Clubhouse>();
        services.AddScoped<IIdentities, Identities>();

        services.Configure<SmtpConnectionOptions>(config.GetSection("Smtp"));
        services.AddSingleton<IEmailValidatorService, EmailValidatorService>();
        services.AddSingleton<SmtpDispatcher>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILoginService, LoginService>();
        

        services.AddHostedService<HostedWorker>();
    }
}
