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
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddSqliteContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder(configuration.GetConnectionString("Sqlite"));
        var file = new FileInfo(connectionStringBuilder.DataSource);
        if (file.Directory?.Exists == false)
        {
            file.Directory.Create();
        }

        var connectionString = connectionStringBuilder.ToString();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.ConfigureWarnings(warn => warn.Ignore(
                CoreEventId.FirstWithoutOrderByAndFilterWarning,
                CoreEventId.RowLimitingOperationWithoutOrderByWarning,
                CoreEventId.DistinctAfterOrderByWithoutRowLimitingOperatorWarning));

            options.UseSqlite(connectionString);
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
        services.AddScoped<IFoodBookings, FoodBookings>();
        services.AddScoped<IFoodInvoices, FoodInvoices>();
        services.AddScoped<ITrips, Trips>();
        services.AddScoped<IVehicles, Vehicles>();
        services.AddScoped<ITryouts, Tryouts>();
        services.AddScoped<IBoats, Boats>();
        services.AddScoped<IClubhouseBookings, ClubhouseBookings>();
        services.AddScoped<IIdentities, Identities>();

        services.Configure<SmtpConnectionOptions>(config.GetSection("Smtp"));
        services.AddSingleton<IEmailValidatorService, EmailValidatorService>();
        services.AddSingleton<IpReputationChecker>();
        services.AddSingleton<SmtpDispatcher>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILoginService, LoginService>();

        services.AddScoped<MySqlMigration>();
        

        services.AddHostedService<HostedWorker>();
    }
}
