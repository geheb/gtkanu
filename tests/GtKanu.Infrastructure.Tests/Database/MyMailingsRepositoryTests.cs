namespace GtKanu.Infrastructure.Tests.Database;

using GtKanu.Application.Models;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Database.Repositories;

public class MyMailingsRepositoryTests : IDisposable
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
    public async Task GetByUser_ShouldReturnOnlyUserMailings()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MyMailingsRepository(timeProvider, context.MyMailings);

        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var mailingId = Guid.NewGuid();

        context.IdentityUsers.Add(CreateTestUser(userId));
        context.IdentityUsers.Add(CreateTestUser(otherUserId));
        context.Mailings.Add(new Mailing { Id = mailingId, Created = timeProvider.GetUtcNow(), Subject = "S", HtmlBody = "B" });
        context.MyMailings.Add(new MyMailing { Id = Guid.NewGuid(), Created = timeProvider.GetUtcNow(), UserId = userId, MailingId = mailingId });
        context.MyMailings.Add(new MyMailing { Id = Guid.NewGuid(), Created = timeProvider.GetUtcNow(), UserId = otherUserId, MailingId = mailingId });
        await context.SaveChangesAsync();

        var result = await repo.GetByUser(userId, CancellationToken.None);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task UpdateHasRead_Existing_ShouldSetHasRead()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MyMailingsRepository(timeProvider, context.MyMailings);

        var id = Guid.NewGuid();
        context.MyMailings.Add(new MyMailing { Id = id, Created = timeProvider.GetUtcNow() });
        await context.SaveChangesAsync();

        var result = await repo.UpdateHasRead(id, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        await context.SaveChangesAsync();

        var entity = await context.MyMailings.FindAsync(id);
        entity!.HasRead.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateHasRead_NonExisting_ShouldFail()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new MyMailingsRepository(timeProvider, context.MyMailings);

        var result = await repo.UpdateHasRead(Guid.NewGuid(), CancellationToken.None);
        result.IsFailed.Should().BeTrue();
    }
}
