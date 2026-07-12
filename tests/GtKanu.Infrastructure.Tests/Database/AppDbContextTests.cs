namespace GtKanu.Infrastructure.Tests.Database;

using GtKanu.Infrastructure.Database;

public class AppDbContextTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public void CreateContext_ShouldNotThrow()
    {
        using var context = _factory.CreateContext();
        context.Should().NotBeNull();
    }

    [Fact]
    public void GeneratePk_ShouldReturnGuid()
    {
        using var context = _factory.CreateContext();
        var pk = context.GeneratePk();
        pk.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void DbSets_ShouldBeAccessible()
    {
        using var context = _factory.CreateContext();
        context.Boats.Should().NotBeNull();
        context.BoatRentals.Should().NotBeNull();
        context.Foods.Should().NotBeNull();
        context.FoodBookings.Should().NotBeNull();
        context.Trips.Should().NotBeNull();
        context.Vehicles.Should().NotBeNull();
        context.WikiArticles.Should().NotBeNull();
        context.EmailQueues.Should().NotBeNull();
    }
}
