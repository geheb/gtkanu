namespace GtKanu.Infrastructure.Tests.Database;

using FluentAssertions;
using GtKanu.Infrastructure.Database.Repositories;

public class UnitOfWorkTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public void Mailings_ShouldReturnInstance()
    {
        using var context = _factory.CreateContext();
        var uow = new UnitOfWork(TimeProvider.System, context);
        uow.Mailings.Should().NotBeNull();
    }

    [Fact]
    public void EmailQueue_ShouldReturnInstance()
    {
        using var context = _factory.CreateContext();
        var uow = new UnitOfWork(TimeProvider.System, context);
        uow.EmailQueue.Should().NotBeNull();
    }

    [Fact]
    public void MyMailings_ShouldReturnInstance()
    {
        using var context = _factory.CreateContext();
        var uow = new UnitOfWork(TimeProvider.System, context);
        uow.MyMailings.Should().NotBeNull();
    }

    [Fact]
    public void WikiArticles_ShouldReturnInstance()
    {
        using var context = _factory.CreateContext();
        var uow = new UnitOfWork(TimeProvider.System, context);
        uow.WikiArticles.Should().NotBeNull();
    }

    [Fact]
    public async Task Save_ShouldPersistChanges()
    {
        using var context = _factory.CreateContext();
        var uow = new UnitOfWork(TimeProvider.System, context);

        uow.Mailings.Create(new() { Id = Guid.NewGuid(), Subject = "Test", Body = "Body" });
        var affected = await uow.Save(CancellationToken.None);
        affected.Should().BeGreaterThan(0);
    }
}
