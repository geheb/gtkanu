namespace GtKanu.Infrastructure.Tests.Database;

using GtKanu.Application.Models;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Database.Repositories;

public class FoodBookingsRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    private static IdentityUserGuid CreateTestUser(Guid id) => new()
    {
        Id = id,
        UserName = id.ToString("N"),
        NormalizedUserName = id.ToString("N").ToUpperInvariant(),
        Email = $"{id}@test.de",
        NormalizedEmail = $"{id}@test.de".ToUpperInvariant(),
        EmailConfirmed = true,
        PasswordHash = "hash",
        SecurityStamp = Guid.NewGuid().ToString(),
        ConcurrencyStamp = Guid.NewGuid().ToString(),
        PhoneNumberConfirmed = false,
        TwoFactorEnabled = false,
        LockoutEnabled = true,
        AccessFailedCount = 0
    };

    [Fact]
    public async Task Create_ShouldAddBooking()
    {
        using var context = _factory.CreateContext();
        var repo = new FoodBookings(context);

        var foodId = Guid.NewGuid();
        context.Foods.Add(new Food { Id = foodId, Name = "Pizza", Price = 8m, Type = (int)FoodType.Dish });

        var userId = Guid.NewGuid();
        context.IdentityUsers.Add(CreateTestUser(userId));
        await context.SaveChangesAsync();

        var result = await repo.Create(userId, foodId, 2, CancellationToken.None);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Create_NonExistingFood_ShouldReturnFalse()
    {
        using var context = _factory.CreateContext();
        var repo = new FoodBookings(context);

        var userId = Guid.NewGuid();
        context.IdentityUsers.Add(CreateTestUser(userId));
        await context.SaveChangesAsync();

        var result = await repo.Create(userId, Guid.NewGuid(), 1, CancellationToken.None);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Cancel_ExistingConfirmedBooking_ShouldSucceed()
    {
        using var context = _factory.CreateContext();
        var repo = new FoodBookings(context);

        var userId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var foodId = Guid.NewGuid();

        context.IdentityUsers.Add(CreateTestUser(userId));
        context.Foods.Add(new Food { Id = foodId, Name = "Burger", Price = 10m, Type = (int)FoodType.Dish });
        context.FoodBookings.Add(new FoodBooking
        {
            Id = bookingId,
            UserId = userId,
            FoodId = foodId,
            Status = (int)BookingStatus.Confirmed,
            Count = 1,
            BookedOn = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        var result = await repo.Cancel(userId, bookingId, CancellationToken.None);
        result.Should().BeTrue();

        var entity = await context.FoodBookings.FindAsync(bookingId);
        entity!.Status.Should().Be((int)BookingStatus.Cancelled);
    }

    [Fact]
    public async Task Cancel_WrongUser_ShouldReturnFalse()
    {
        using var context = _factory.CreateContext();
        var repo = new FoodBookings(context);

        var userId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var foodId = Guid.NewGuid();

        context.IdentityUsers.Add(CreateTestUser(userId));
        context.Foods.Add(new Food { Id = foodId, Name = "Burger", Price = 10m, Type = (int)FoodType.Dish });
        context.FoodBookings.Add(new FoodBooking
        {
            Id = bookingId,
            UserId = userId,
            FoodId = foodId,
            Status = (int)BookingStatus.Confirmed,
            Count = 1,
            BookedOn = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        var result = await repo.Cancel(Guid.NewGuid(), bookingId, CancellationToken.None);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Complete_ExistingConfirmedBooking_ShouldSucceed()
    {
        using var context = _factory.CreateContext();
        var repo = new FoodBookings(context);

        var bookingId = Guid.NewGuid();
        var foodId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        context.IdentityUsers.Add(CreateTestUser(userId));
        context.Foods.Add(new Food { Id = foodId, Name = "Burger", Price = 10m, Type = (int)FoodType.Dish });
        context.FoodBookings.Add(new FoodBooking
        {
            Id = bookingId,
            UserId = userId,
            FoodId = foodId,
            Status = (int)BookingStatus.Confirmed,
            Count = 1,
            BookedOn = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        var result = await repo.Complete(bookingId, CancellationToken.None);
        result.Should().BeTrue();

        var entity = await context.FoodBookings.FindAsync(bookingId);
        entity!.Status.Should().Be((int)BookingStatus.Completed);
    }

    [Fact]
    public async Task Complete_AlreadyCompleted_ShouldReturnFalse()
    {
        using var context = _factory.CreateContext();
        var repo = new FoodBookings(context);

        var bookingId = Guid.NewGuid();
        var foodId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        context.IdentityUsers.Add(CreateTestUser(userId));
        context.Foods.Add(new Food { Id = foodId, Name = "Burger", Price = 10m, Type = (int)FoodType.Dish });
        context.FoodBookings.Add(new FoodBooking
        {
            Id = bookingId,
            UserId = userId,
            FoodId = foodId,
            Status = (int)BookingStatus.Completed,
            Count = 1,
            BookedOn = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        var result = await repo.Complete(bookingId, CancellationToken.None);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetInvoiceBookings_ShouldReturnBookingsForInvoice()
    {
        using var context = _factory.CreateContext();
        var repo = new FoodBookings(context);

        var invoiceId = Guid.NewGuid();
        var foodId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        context.IdentityUsers.Add(CreateTestUser(userId));
        context.Foods.Add(new Food { Id = foodId, Name = "Burger", Price = 10m, Type = (int)FoodType.Dish });
        context.FoodInvoices.Add(new FoodInvoice
        {
            Id = invoiceId,
            UserId = userId,
            CreatedOn = DateTimeOffset.UtcNow,
            Total = 20m,
            Status = (int)InvoiceStatus.Open
        });
        context.FoodBookings.Add(new FoodBooking
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FoodId = foodId,
            InvoiceId = invoiceId,
            Status = (int)BookingStatus.Confirmed,
            Count = 2,
            BookedOn = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        var result = await repo.GetInvoiceBookings(invoiceId, CancellationToken.None);
        result.Should().ContainSingle();
    }
}
