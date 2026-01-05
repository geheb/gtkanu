using GtKanu.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace GtKanu.Infrastructure.Database;

internal sealed class AppDbContext :
    IdentityDbContext<IdentityUserGuid, IdentityRoleGuid, Guid, IdentityUserClaimGuid, IdentityUserRoleGuid, IdentityUserLoginGuid, IdentityRoleClaimGuid, IdentityUserTokenGuid>
{
    const string KeyType = "binary(16)";
    public DbSet<Boat> Boats => Set<Boat>();
    public DbSet<BoatRental> BoatRentals => Set<BoatRental>();
    public DbSet<Food> Foods => Set<Food>();
    public DbSet<FoodList> FoodLists => Set<FoodList>();
    public DbSet<Booking> FoodBookings => Set<Booking>();
    public DbSet<ClubhouseBooking> ClubhouseBookings => Set<ClubhouseBooking>();
    public DbSet<EmailQueue> EmailQueues => Set<EmailQueue>();
    public DbSet<IdentityUserGuid> Identities => Set<IdentityUserGuid>();
    public DbSet<Invoice> FoodInvoices => Set<Invoice>();
    public DbSet<InvoicePeriod> FoodInvoicePeriods => Set<InvoicePeriod>();
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasCharSet(CharSet.Utf8Mb4, DelegationModes.ApplyToDatabases);

        modelBuilder.Entity<IdentityUserGuid>(eb =>
        {
            eb.Property(e => e.Id).HasColumnType(KeyType);
            eb.Property(e => e.Name).HasMaxLength(256);
            eb.Property(e => e.DebtorNumber).HasMaxLength(256);
            eb.Property(e => e.AddressNumber).HasMaxLength(256);

            eb.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            eb.ToTable("users");
        });

        modelBuilder.Entity<IdentityRoleGuid>(eb =>
        {
            eb.Property(e => e.Id).HasColumnType(KeyType);

            eb.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(e => e.RoleId)
                .IsRequired();

            eb.ToTable("roles");
        });

        modelBuilder.Entity<IdentityUserRoleGuid>(eb =>
        {
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.RoleId).HasColumnType(KeyType);
            eb.ToTable("user_roles");
        });

        modelBuilder.Entity<IdentityUserLoginGuid>(eb =>
        {
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.ToTable("user_logins");
        });

        modelBuilder.Entity<IdentityUserTokenGuid>(eb =>
        {
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.ToTable("user_tokens");
        });

        modelBuilder.Entity<IdentityUserClaimGuid>(eb =>
        {
            eb.Property(e => e.Id).UseMySqlIdentityColumn();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.ToTable("user_claims");
        });

        modelBuilder.Entity<IdentityRoleClaimGuid>(eb =>
        {
            eb.Property(e => e.Id).UseMySqlIdentityColumn();
            eb.Property(e => e.RoleId).HasColumnType(KeyType);
            eb.ToTable("role_claims");
        });

        modelBuilder.ApplyConfiguration(new RoleSeeder());

        modelBuilder.Entity<EmailQueue>(eb =>
        {
            eb.ToTable("email_queue");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Created).IsRequired();
            eb.Property(e => e.Recipient).IsRequired();
            eb.Property(e => e.Subject).IsRequired();
            eb.Property(e => e.HtmlBody).IsRequired();

            eb.HasIndex(e => new { e.NextSchedule, e.Sent, e.IsPrio, e.Created });
        });

        modelBuilder.Entity<Food>(eb =>
        {
            eb.ToTable("foods");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Name).IsRequired().HasMaxLength(128);
            eb.Property(e => e.Price).HasColumnType("decimal(6,2)").IsRequired();
            eb.Property(e => e.Type).IsRequired();
            eb.Property(e => e.FoodListId).HasColumnType(KeyType);

            eb.HasOne(e => e.FoodList)
                .WithMany(e => e.Foods)
                .HasForeignKey(e => e.FoodListId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<FoodList>(eb =>
        {
            eb.ToTable("food_lists");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Name).IsRequired().HasMaxLength(128);
            eb.Property(e => e.ValidFrom).IsRequired();

            eb.HasIndex(e => e.ValidFrom);
        });

        modelBuilder.Entity<Booking>(eb =>
        {
            eb.ToTable("bookings");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.FoodId).HasColumnType(KeyType);
            eb.Property(e => e.Status).IsRequired();
            eb.Property(e => e.Count).IsRequired();
            eb.Property(e => e.BookedOn).IsRequired();
            eb.Property(e => e.InvoiceId).HasColumnType(KeyType);

            eb.HasOne(e => e.User)
                .WithMany(e => e.Bookings)
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

        modelBuilder.Entity<Invoice>(eb =>
        {
            eb.ToTable("invoices");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.CreatedOn).IsRequired();
            eb.Property(e => e.Total).HasColumnType("decimal(6,2)").IsRequired();
            eb.Property(e => e.Status).IsRequired();

            eb.HasOne(e => e.InvoicePeriod)
                .WithMany(e => e.Invoices)
                .HasForeignKey(e => e.InvoicePeriodId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            eb.HasOne(e => e.User)
                .WithMany(e => e.Invoices)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        });

        modelBuilder.Entity<InvoicePeriod>(eb =>
        {
            eb.ToTable("invoice_periods");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Description).IsRequired();
            eb.Property(e => e.From).IsRequired();
            eb.Property(e => e.To).IsRequired();

            eb.HasIndex(e => e.To);
        });

        modelBuilder.Entity<Trip>(eb =>
        {
            eb.ToTable("trips");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
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
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.TripId).HasColumnType(KeyType);
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
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
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
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.TripId).HasColumnType(KeyType);
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
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
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
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.VehicleId).HasColumnType(KeyType);
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
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Type).IsRequired().HasDefaultValue("Anfängertraining").HasMaxLength(64);
            eb.Property(e => e.UserId).HasColumnType(KeyType);
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
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.TryoutId).HasColumnType(KeyType);
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
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
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
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.BoatId).HasColumnType(KeyType);
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
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Start).IsRequired();
            eb.Property(e => e.End).IsRequired();
            eb.Property(e => e.Title).IsRequired().HasMaxLength(128);
        });

        modelBuilder.Entity<TryoutChat>(eb =>
        {
            eb.ToTable("tryout_chats");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.UserId).HasColumnType(KeyType);
            eb.Property(e => e.TryoutId).HasColumnType(KeyType);
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
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
            eb.Property(e => e.Created).IsRequired();
            eb.Property(e => e.Subject).IsRequired();
            eb.Property(e => e.HtmlBody).IsRequired();
        });

        modelBuilder.Entity<MyMailing>(eb =>
        {
            eb.ToTable("my_mailings");
            eb.Property(e => e.Id).HasColumnType(KeyType).ValueGeneratedNever();
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
}
