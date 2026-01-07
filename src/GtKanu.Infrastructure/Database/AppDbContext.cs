using GtKanu.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GtKanu.Infrastructure.Database;

internal sealed class AppDbContext :
    IdentityDbContext<IdentityUserGuid, IdentityRoleGuid, Guid, IdentityUserClaimGuid, IdentityUserRoleGuid, IdentityUserLoginGuid, IdentityRoleClaimGuid, IdentityUserTokenGuid>
{
    public DbSet<Boat> Boats => Set<Boat>();
    public DbSet<BoatRental> BoatRentals => Set<BoatRental>();
    public DbSet<Food> Foods => Set<Food>();
    public DbSet<FoodList> FoodLists => Set<FoodList>();
    public DbSet<FoodBooking> FoodBookings => Set<FoodBooking>();
    public DbSet<ClubhouseBooking> ClubhouseBookings => Set<ClubhouseBooking>();
    public DbSet<EmailQueue> EmailQueues => Set<EmailQueue>();
    public DbSet<IdentityUserGuid> IdentityUsers => Set<IdentityUserGuid>();
    public DbSet<FoodInvoice> FoodInvoices => Set<FoodInvoice>();
    public DbSet<FoodInvoicePeriod> FoodInvoicePeriods => Set<FoodInvoicePeriod>();
    public DbSet<Mailing> Mailings => Set<Mailing>();
    public DbSet<MyMailing> MyMailings => Set<MyMailing>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<TripBooking> TripBookings => Set<TripBooking>();
    public DbSet<TripChat> TripChats => Set<TripChat>();
    public DbSet<Tryout> Tryouts => Set<Tryout>();
    public DbSet<TryoutBooking> TryoutBookings => Set<TryoutBooking>();
    public DbSet<TryoutChat> TryoutChats => Set<TryoutChat>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleBooking> VehicleBookings => Set<VehicleBooking>();
    public DbSet<WikiArticle> WikiArticles => Set<WikiArticle>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public Guid GeneratePk() => Guid.CreateVersion7();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Guid>()
            .HaveMaxLength(32);

        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetToUtcConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityUserGuid>(eb =>
        {
            eb.Property(e => e.Name).HasMaxLength(256);
            eb.Property(e => e.DebtorNumber).HasMaxLength(256);
            eb.Property(e => e.AddressNumber).HasMaxLength(256);

            eb.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            eb.ToTable("identity_users");
        });

        modelBuilder.Entity<IdentityRoleGuid>(eb =>
        {
            eb.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .IsRequired();

            eb.ToTable("identity_roles");
        });

        modelBuilder.Entity<IdentityUserRoleGuid>(eb =>
        {
            eb.ToTable("identity_user_roles");
        });

        modelBuilder.Entity<IdentityUserLoginGuid>(eb =>
        {
            eb.ToTable("identity_user_logins");
        });

        modelBuilder.Entity<IdentityUserTokenGuid>(eb =>
        {
            eb.ToTable("identity_user_tokens");
        });

        modelBuilder.Entity<IdentityUserClaimGuid>(eb =>
        {
            eb.Property(e => e.Id).ValueGeneratedOnAdd();
            eb.ToTable("identity_user_claims");
        });

        modelBuilder.Entity<IdentityRoleClaimGuid>(eb =>
        {
            eb.Property(e => e.Id).ValueGeneratedOnAdd();
            eb.ToTable("identity_role_claims");
        });

        modelBuilder.ApplyConfiguration(new RoleSeeder());

        modelBuilder.Entity<EmailQueue>(eb =>
        {
            eb.ToTable("email_queue");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Created).IsRequired();
            eb.Property(e => e.Recipient).IsRequired();
            eb.Property(e => e.Subject).IsRequired();
            eb.Property(e => e.HtmlBody).IsRequired();

            eb.HasIndex(e => new { e.NextSchedule, e.Sent, e.IsPrio, e.Created });
        });

        modelBuilder.Entity<Food>(eb =>
        {
            eb.ToTable("foods");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Name).IsRequired().HasMaxLength(128);
            eb.Property(e => e.Price).IsRequired();
            eb.Property(e => e.Type).IsRequired();

            eb.HasOne(e => e.FoodList)
                .WithMany(e => e.Foods)
                .HasForeignKey(e => e.FoodListId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<FoodList>(eb =>
        {
            eb.ToTable("food_lists");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Name).IsRequired().HasMaxLength(128);
            eb.Property(e => e.ValidFrom).IsRequired();

            eb.HasIndex(e => e.ValidFrom);
        });

        modelBuilder.Entity<FoodBooking>(eb =>
        {
            eb.ToTable("food_bookings");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Status).IsRequired();
            eb.Property(e => e.Count).IsRequired();
            eb.Property(e => e.BookedOn).IsRequired();

            eb.HasOne(e => e.User)
                .WithMany(e => e.FoodBookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Food)
                .WithMany(e => e.Bookings)
                .HasForeignKey(e => e.FoodId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Invoice)
                .WithMany(e => e.Bookings)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasIndex(e => new { e.UserId, e.BookedOn});
        });

        modelBuilder.Entity<FoodInvoice>(eb =>
        {
            eb.ToTable("food_invoices");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.CreatedOn).IsRequired();
            eb.Property(e => e.Total).HasColumnType("decimal(6,2)").IsRequired();
            eb.Property(e => e.Status).IsRequired();

            eb.HasOne(e => e.InvoicePeriod)
                .WithMany(e => e.Invoices)
                .HasForeignKey(e => e.InvoicePeriodId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.User)
                .WithMany(e => e.FoodInvoices)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<FoodInvoicePeriod>(eb =>
        {
            eb.ToTable("food_invoice_periods");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Description).IsRequired();
            eb.Property(e => e.From).IsRequired();
            eb.Property(e => e.To).IsRequired();

            eb.HasIndex(e => e.To);
        });

        modelBuilder.Entity<Trip>(eb =>
        {
            eb.ToTable("trips");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Start).IsRequired();
            eb.Property(e => e.End).IsRequired();
            eb.Property(e => e.Target).IsRequired();
            eb.Property(e => e.MaxBookings).IsRequired();
            eb.Property(e => e.BookingStart).IsRequired();
            eb.Property(e => e.BookingEnd).IsRequired();

            eb.HasOne(e => e.User)
                .WithMany(e => e.Trips)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasIndex(e => new { e.Start, e.End });
        });

        modelBuilder.Entity<TripBooking>(eb =>
        {
            eb.ToTable("trip_bookings");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.BookedOn).IsRequired();
            eb.Property(e => e.Name).HasMaxLength(256);

            eb.HasOne(e => e.User)
                .WithMany(e => e.TripBookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Trip)
                .WithMany(e => e.TripBookings)
                .HasForeignKey(e => e.TripId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<WikiArticle>(eb =>
        {
            eb.ToTable("wiki_articles");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Created).IsRequired();
            eb.Property(e => e.Identifier).IsRequired().HasMaxLength(16);
            eb.Property(e => e.Title).IsRequired().HasMaxLength(256);
            eb.Property(e => e.Content).IsRequired();

            eb.HasOne(e => e.User)
                .WithMany(e => e.WikiArticles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasIndex(e => e.Identifier).IsUnique();
        });

        modelBuilder.Entity<TripChat>(eb =>
        {
            eb.ToTable("trip_chats");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.CreatedOn).IsRequired();
            eb.Property(e => e.Message).IsRequired().HasMaxLength(256);

            eb.HasOne(e => e.User)
                .WithMany(e => e.TripChats)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Trip)
                .WithMany(e => e.TripChats)
                .HasForeignKey(e => e.TripId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<Vehicle>(eb =>
        {
            eb.ToTable("vehicles");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Name).IsRequired().HasMaxLength(64);
            eb.Property(e => e.Identifier).IsRequired().HasMaxLength(12);

            eb.HasMany(e => e.Bookings)
                .WithOne(e => e.Vehicle)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<VehicleBooking>(eb =>
        {
            eb.ToTable("vehicle_bookings");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Start).IsRequired();
            eb.Property(e => e.End).IsRequired();
            eb.Property(e => e.BookedOn).IsRequired();
            eb.Property(e => e.Purpose).IsRequired().HasMaxLength(128);

            eb.HasOne(e => e.User)
                .WithMany(e => e.VehicleBookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Vehicle)
                .WithMany(e => e.Bookings)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<Tryout>(eb =>
        {
            eb.ToTable("tryouts");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Type).IsRequired().HasDefaultValue("Anfängertraining").HasMaxLength(64);
            eb.Property(e => e.Date).IsRequired();
            eb.Property(e => e.MaxBookings).IsRequired();
            eb.Property(e => e.BookingStart).IsRequired();
            eb.Property(e => e.BookingEnd).IsRequired();
            
            eb.HasOne(e => e.User)
                .WithMany(e => e.Tryouts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<TryoutBooking>(eb =>
        {
            eb.ToTable("tryout_bookings");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.BookedOn).IsRequired();
            eb.Property(e => e.Name).HasMaxLength(256);

            eb.HasOne(e => e.User)
                .WithMany(e => e.TryoutBookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Tryout)
                .WithMany(e => e.TryoutBookings)
                .HasForeignKey(e => e.TryoutId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<Boat>(eb =>
        {
            eb.ToTable("boats");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Name).IsRequired().HasMaxLength(64);
            eb.Property(e => e.Identifier).IsRequired().HasMaxLength(8);
            eb.Property(e => e.Location).IsRequired().HasMaxLength(64);

            eb.HasMany(e => e.BoatRentals)
                .WithOne(e => e.Boat)
                .HasForeignKey(e => e.BoatId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<BoatRental>(eb =>
        {
            eb.ToTable("boat_rentals");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Start).IsRequired();
            eb.Property(e => e.End).IsRequired();
            eb.Property(e => e.Purpose).IsRequired().HasMaxLength(128);

            eb.HasOne(e => e.User)
                .WithMany(e => e.BoatRentals)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Boat)
                .WithMany(e => e.BoatRentals)
                .HasForeignKey(e => e.BoatId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<ClubhouseBooking>(eb =>
        {
            eb.ToTable("clubhouse_bookings");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Start).IsRequired();
            eb.Property(e => e.End).IsRequired();
            eb.Property(e => e.Title).IsRequired().HasMaxLength(128);
        });

        modelBuilder.Entity<TryoutChat>(eb =>
        {
            eb.ToTable("tryout_chats");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.CreatedOn).IsRequired();
            eb.Property(e => e.Message).IsRequired().HasMaxLength(256);

            eb.HasOne(e => e.User)
                .WithMany(e => e.TryoutChats)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.Tryout)
                .WithMany(e => e.TryoutChats)
                .HasForeignKey(e => e.TryoutId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<Mailing>(eb =>
        {
            eb.ToTable("mailings");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Created).IsRequired();
            eb.Property(e => e.Subject).IsRequired();
            eb.Property(e => e.HtmlBody).IsRequired();
        });

        modelBuilder.Entity<MyMailing>(eb =>
        {
            eb.ToTable("my_mailings");
            eb.Property(e => e.Id).ValueGeneratedNever();
            eb.Property(e => e.Created).IsRequired();

            eb.HasOne(e => e.Mailing)
                .WithMany(e => e.MyMailings)
                .HasForeignKey(e => e.MailingId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.User)
                .WithMany(e => e.MyMailings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });
    }

    private sealed class DateTimeOffsetToUtcConverter : ValueConverter<DateTimeOffset, DateTime>
    {
        public DateTimeOffsetToUtcConverter() :
            base(v => v.UtcDateTime, v => new DateTimeOffset(v, TimeSpan.Zero))
        {
        }
    }
}
