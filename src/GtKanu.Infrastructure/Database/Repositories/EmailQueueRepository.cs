using FluentResults;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace GtKanu.Infrastructure.Database.Repositories;

internal sealed class EmailQueueRepository : Repository<EmailQueue, EmailQueueDto>, IEmailQueues
{
    public EmailQueueRepository(TimeProvider timeProvider, DbSet<EmailQueue> dbSet) 
        : base(timeProvider, dbSet)
    {
    }

    public async Task<EmailQueueDto[]> GetNextToSend(int count, CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow();

        var entities = await _dbSet
            .AsNoTracking()
            .Where(e => (e.NextSchedule <= now || e.NextSchedule == null) && e.Sent == null)
            .OrderByDescending(e => e.IsPrio).ThenBy(e => e.Created)
            .Select(e => new { e.Id, e.Subject, e.Recipient, e.HtmlBody, e.ReplyAddress })
            .Take(count)
            .ToArrayAsync(cancellationToken);

        if (entities.Length == 0)
        {
            return [];
        }

        var result = new List<EmailQueueDto>();

        foreach (var e in entities)
        {
            result.Add(new() 
            { 
                Id = e.Id, 
                Subject = e.Subject, 
                Recipient = e.Recipient, 
                HtmlBody = e.HtmlBody,
                ReplyAddress = e.ReplyAddress
            });
        }

        return [.. result];
    }

    public async Task<Result> UpdateSent(Guid[] ids, CancellationToken cancellationToken)
    {
        var entities = await _dbSet
            .Where(e => ids.Contains(e.Id))
            .ToArrayAsync(cancellationToken);

        if (entities.Length == 0)
        {
            return Result.Fail("Keine Datensätze gefunden.");
        }

        var now = _timeProvider.GetUtcNow();
        foreach (var e in entities)
        {
            e.LastError = null;
            e.Sent = now;
        }

        return Result.Ok();
    }

    public async Task<Result> UpdateNextSchedule(Guid id, string lastError, CancellationToken cancellationToken)
    {
        var entity = await _dbSet
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity is null)
        {
            return NotFound;
        }

        entity.LastError = lastError;
        entity.NextSchedule = _timeProvider.GetUtcNow().AddHours(1);

        return Result.Ok();
    }
}
