namespace GtKanu.Infrastructure.Tests.Database;

using GtKanu.Application.Models;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Database.Repositories;

public class BoatsRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Create_ShouldAddBoat()
    {
        using var context = _factory.CreateContext();
        var boats = new Boats(context);

        var dto = new BoatDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Boat",
            Identifier = "TB01",
            Location = "Dock A",
            MaxRentalDays = 3
        };

        var result = await boats.Create(dto, CancellationToken.None);
        result.Should().Be(BoatStatus.Success);
        dto.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Create_DuplicateIdentifier_ShouldReturnExists()
    {
        using var context = _factory.CreateContext();
        var boats = new Boats(context);

        await boats.Create(new BoatDto { Id = Guid.NewGuid(), Name = "B1", Identifier = "DUP", Location = "L1", MaxRentalDays = 1 }, CancellationToken.None);
        var result = await boats.Create(new BoatDto { Id = Guid.NewGuid(), Name = "B2", Identifier = "DUP", Location = "L2", MaxRentalDays = 1 }, CancellationToken.None);
        result.Should().Be(BoatStatus.Exists);
    }

    [Fact]
    public async Task FindBoat_ExistingId_ShouldReturnBoat()
    {
        using var context = _factory.CreateContext();
        var boats = new Boats(context);

        var id = Guid.NewGuid();
        context.Boats.Add(new Boat { Id = id, Name = "FindMe", Identifier = "F01", Location = "L", MaxRentalDays = 1 });
        await context.SaveChangesAsync();

        var found = await boats.FindBoat(id, CancellationToken.None);
        found.Should().NotBeNull();
        found!.Name.Should().Be("FindMe");
    }

    [Fact]
    public async Task FindBoat_NonExistingId_ShouldReturnNull()
    {
        using var context = _factory.CreateContext();
        var boats = new Boats(context);

        var found = await boats.FindBoat(Guid.NewGuid(), CancellationToken.None);
        found.Should().BeNull();
    }

    [Fact]
    public async Task Update_ExistingBoat_ShouldSucceed()
    {
        using var context = _factory.CreateContext();
        var boats = new Boats(context);

        var id = Guid.NewGuid();
        context.Boats.Add(new Boat { Id = id, Name = "Old", Identifier = "U01", Location = "L", MaxRentalDays = 1 });
        await context.SaveChangesAsync();

        var result = await boats.Update(new BoatDto { Id = id, Name = "New", Identifier = "U01", Location = "L", MaxRentalDays = 1 }, CancellationToken.None);
        result.Should().Be(BoatStatus.Success);

        var found = await boats.FindBoat(id, CancellationToken.None);
        found!.Name.Should().Be("New");
    }

    [Fact]
    public async Task Update_NonExistingBoat_ShouldReturnNotFound()
    {
        using var context = _factory.CreateContext();
        var boats = new Boats(context);

        var result = await boats.Update(new BoatDto { Id = Guid.NewGuid(), Name = "X", Identifier = "X01", Location = "L", MaxRentalDays = 1 }, CancellationToken.None);
        result.Should().Be(BoatStatus.NotFound);
    }

    [Fact]
    public async Task GetRentalList_ShouldReturnBoatsWithCount()
    {
        using var context = _factory.CreateContext();
        var boats = new Boats(context);

        var boatId = Guid.NewGuid();
        context.Boats.Add(new Boat { Id = boatId, Name = "B1", Identifier = "R01", Location = "L", MaxRentalDays = 1 });
        context.BoatRentals.Add(new BoatRental
        {
            Id = Guid.NewGuid(),
            BoatId = boatId,
            Start = DateTimeOffset.UtcNow,
            End = DateTimeOffset.UtcNow.AddDays(1),
            Purpose = "Test"
        });
        await context.SaveChangesAsync();

        var list = await boats.GetRentalList(CancellationToken.None);
        list.Should().ContainSingle();
        list[0].Boat.Should().NotBeNull();
        list[0].Boat!.Name.Should().Be("B1");
        list[0].Count.Should().Be(1);
    }

    [Fact]
    public async Task CancelRental_ExistingRental_ShouldSetCancelledOn()
    {
        using var context = _factory.CreateContext();
        var boats = new Boats(context);

        var rentalId = Guid.NewGuid();
        context.BoatRentals.Add(new BoatRental
        {
            Id = rentalId,
            Start = DateTimeOffset.UtcNow,
            End = DateTimeOffset.UtcNow.AddDays(1),
            Purpose = "Test"
        });
        await context.SaveChangesAsync();

        var result = await boats.CancelRental(rentalId, CancellationToken.None);
        result.Should().BeTrue();

        var rental = await context.BoatRentals.FindAsync(rentalId);
        rental!.CancelledOn.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelRental_AlreadyCancelled_ShouldReturnFalse()
    {
        using var context = _factory.CreateContext();
        var boats = new Boats(context);

        var rentalId = Guid.NewGuid();
        context.BoatRentals.Add(new BoatRental
        {
            Id = rentalId,
            Start = DateTimeOffset.UtcNow,
            End = DateTimeOffset.UtcNow.AddDays(1),
            Purpose = "Test",
            CancelledOn = DateTimeOffset.UtcNow
        });
        await context.SaveChangesAsync();

        var result = await boats.CancelRental(rentalId, CancellationToken.None);
        result.Should().BeFalse();
    }
}
