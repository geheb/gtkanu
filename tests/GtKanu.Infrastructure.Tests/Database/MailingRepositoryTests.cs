namespace GtKanu.Infrastructure.Tests.Database;

using GtKanu.Application.Models;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Database.Repositories;

public class MailingRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Create_ShouldAddMailing()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MailingRepository(timeProvider, context.Mailings);

        var dto = new MailingDto
        {
            Id = Guid.NewGuid(),
            Subject = "Test",
            Body = "Body",
            IsMemberOnly = true
        };

        repo.Create(dto);
        await context.SaveChangesAsync();

        var all = await repo.GetAll(CancellationToken.None);
        all.Should().ContainSingle();
        all[0].Subject.Should().Be("Test");
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllMailings()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MailingRepository(timeProvider, context.Mailings);

        context.Mailings.Add(new Mailing { Id = Guid.NewGuid(), Created = timeProvider.GetUtcNow(), Subject = "A", HtmlBody = "A" });
        context.Mailings.Add(new Mailing { Id = Guid.NewGuid(), Created = timeProvider.GetUtcNow(), Subject = "B", HtmlBody = "B" });
        await context.SaveChangesAsync();

        var all = await repo.GetAll(CancellationToken.None);
        all.Should().HaveCount(2);
    }

    [Fact]
    public async Task Find_ExistingId_ShouldReturnMailing()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MailingRepository(timeProvider, context.Mailings);

        var id = Guid.NewGuid();
        context.Mailings.Add(new Mailing { Id = id, Created = timeProvider.GetUtcNow(), Subject = "FindMe", HtmlBody = "Body" });
        await context.SaveChangesAsync();

        var found = await repo.Find(id, CancellationToken.None);
        found.Should().NotBeNull();
        found!.Value.Subject.Should().Be("FindMe");
    }

    [Fact]
    public async Task Find_NonExistingId_ShouldReturnNull()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MailingRepository(timeProvider, context.Mailings);

        var found = await repo.Find(Guid.NewGuid(), CancellationToken.None);
        found.Should().BeNull();
    }

    [Fact]
    public async Task Update_ExistingMailing_ShouldSucceed()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MailingRepository(timeProvider, context.Mailings);

        var id = Guid.NewGuid();
        context.Mailings.Add(new Mailing { Id = id, Created = timeProvider.GetUtcNow(), Subject = "Old", HtmlBody = "Body" });
        await context.SaveChangesAsync();

        var result = await repo.Update(new MailingDto { Id = id, Subject = "New", Body = "Body" }, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        await context.SaveChangesAsync();

        var found = await repo.Find(id, CancellationToken.None);
        found!.Value.Subject.Should().Be("New");
    }

    [Fact]
    public async Task Update_NonExistingMailing_ShouldFail()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MailingRepository(timeProvider, context.Mailings);

        var result = await repo.Update(new MailingDto { Id = Guid.NewGuid(), Subject = "X", Body = "Y" }, CancellationToken.None);
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_ExistingMailing_ShouldSucceed()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MailingRepository(timeProvider, context.Mailings);

        var id = Guid.NewGuid();
        context.Mailings.Add(new Mailing { Id = id, Created = timeProvider.GetUtcNow(), Subject = "Del", HtmlBody = "Body" });
        await context.SaveChangesAsync();

        var result = await repo.Delete(id, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        await context.SaveChangesAsync();

        var found = await repo.Find(id, CancellationToken.None);
        found.Should().BeNull();
    }

    [Fact]
    public async Task Delete_NonExistingMailing_ShouldFail()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MailingRepository(timeProvider, context.Mailings);

        var result = await repo.Delete(Guid.NewGuid(), CancellationToken.None);
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateClosed_ShouldSetClosedAndEmailCount()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MailingRepository(timeProvider, context.Mailings);

        var id = Guid.NewGuid();
        context.Mailings.Add(new Mailing { Id = id, Created = timeProvider.GetUtcNow(), Subject = "S", HtmlBody = "B" });
        await context.SaveChangesAsync();

        var result = await repo.UpdateClosed(id, 42, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        await context.SaveChangesAsync();

        var found = await context.Mailings.FindAsync(id);
        found.Should().NotBeNull();
        found!.IsClosed.Should().BeTrue();
        found.EmailCount.Should().Be(42);
    }

    [Fact]
    public async Task UpdateClosed_NonExisting_ShouldFail()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MailingRepository(timeProvider, context.Mailings);

        var result = await repo.UpdateClosed(Guid.NewGuid(), 0, CancellationToken.None);
        result.IsFailed.Should().BeTrue();
    }
}
