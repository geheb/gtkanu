namespace GtKanu.Infrastructure.Tests.Database;

using GtKanu.Application.Models;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Database.Repositories;

public class EmailQueueRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task GetNextToSend_ShouldReturnOnlyUnsentItems()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new EmailQueueRepository(timeProvider, context.EmailQueues);

        var now = timeProvider.GetUtcNow();

        context.EmailQueues.Add(new EmailQueue
        {
            Id = Guid.NewGuid(),
            Created = now,
            Recipient = "a@a.de",
            Subject = "S1",
            HtmlBody = "B1",
            NextSchedule = now.AddMinutes(-1)
        });
        context.EmailQueues.Add(new EmailQueue
        {
            Id = Guid.NewGuid(),
            Created = now,
            Recipient = "b@b.de",
            Subject = "S2",
            HtmlBody = "B2",
            Sent = now
        });
        await context.SaveChangesAsync();

        var result = await repo.GetNextToSend(10, CancellationToken.None);
        result.Should().ContainSingle();
        result[0].Subject.Should().Be("S1");
    }

    [Fact]
    public async Task GetNextToSend_ShouldRespectCount()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new EmailQueueRepository(timeProvider, context.EmailQueues);

        var now = timeProvider.GetUtcNow();
        for (int i = 0; i < 5; i++)
        {
            context.EmailQueues.Add(new EmailQueue
            {
                Id = Guid.NewGuid(),
                Created = now,
                Recipient = $"{i}@a.de",
                Subject = $"S{i}",
                HtmlBody = "B",
                NextSchedule = now.AddMinutes(-1)
            });
        }
        await context.SaveChangesAsync();

        var result = await repo.GetNextToSend(2, CancellationToken.None);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CountSentByCorrelationId_ShouldReturnCorrectCount()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new EmailQueueRepository(timeProvider, context.EmailQueues);

        var correlationId = Guid.NewGuid();
        var now = timeProvider.GetUtcNow();

        context.EmailQueues.Add(new EmailQueue { Id = Guid.NewGuid(), Created = now, Recipient = "a", Subject = "S", HtmlBody = "B", CorrelationId = correlationId, Sent = now });
        context.EmailQueues.Add(new EmailQueue { Id = Guid.NewGuid(), Created = now, Recipient = "b", Subject = "S", HtmlBody = "B", CorrelationId = correlationId });
        context.EmailQueues.Add(new EmailQueue { Id = Guid.NewGuid(), Created = now, Recipient = "c", Subject = "S", HtmlBody = "B", Sent = now });
        await context.SaveChangesAsync();

        var count = await repo.CountSentByCorrelationId(correlationId, CancellationToken.None);
        count.Should().Be(1);
    }

    [Fact]
    public async Task UpdateSent_ShouldSetSentDate()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new EmailQueueRepository(timeProvider, context.EmailQueues);

        var id = Guid.NewGuid();
        var now = timeProvider.GetUtcNow();
        context.EmailQueues.Add(new EmailQueue { Id = id, Created = now, Recipient = "a", Subject = "S", HtmlBody = "B" });
        await context.SaveChangesAsync();

        var result = await repo.UpdateSent([id], CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        await context.SaveChangesAsync();

        var entity = await context.EmailQueues.FindAsync(id);
        entity!.Sent.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateSent_EmptyArray_ShouldFail()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new EmailQueueRepository(timeProvider, context.EmailQueues);

        var result = await repo.UpdateSent([], CancellationToken.None);
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateNextSchedule_ShouldUpdateScheduleAndError()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new EmailQueueRepository(timeProvider, context.EmailQueues);

        var id = Guid.NewGuid();
        var now = timeProvider.GetUtcNow();
        context.EmailQueues.Add(new EmailQueue { Id = id, Created = now, Recipient = "a", Subject = "S", HtmlBody = "B" });
        await context.SaveChangesAsync();

        var result = await repo.UpdateNextSchedule(id, "Error", CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        await context.SaveChangesAsync();

        var entity = await context.EmailQueues.FindAsync(id);
        entity!.LastError.Should().Be("Error");
        entity.NextSchedule.Should().BeAfter(now);
    }

    [Fact]
    public async Task UpdateNextSchedule_NonExisting_ShouldFail()
    {
        using var context = _factory.CreateContext();
        var timeProvider = TimeProvider.System;
        var repo = new EmailQueueRepository(timeProvider, context.EmailQueues);

        var result = await repo.UpdateNextSchedule(Guid.NewGuid(), "Error", CancellationToken.None);
        result.IsFailed.Should().BeTrue();
    }
}
