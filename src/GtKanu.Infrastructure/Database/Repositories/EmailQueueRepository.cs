using FluentResults;
using GtKanu.Application.Converter;
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
            .Where(e => (e.NextSchedule == null || e.NextSchedule <= now) && e.Sent == null && (e.RetryCount == null || e.RetryCount < 7))
            .OrderByDescending(e => e.IsPrio).ThenBy(e => e.Created)
            .Take(count)
            .ToArrayAsync(cancellationToken);

        if (entities.Length == 0)
        {
            return [];
        }

        var result = new List<EmailQueueDto>();
        var dc = new GermanDateTimeConverter();

        foreach (var e in entities)
        {
            result.Add(e.ToDto(dc));
        }

        return [.. result];
    }

    public async Task<int> CountSentByCorrelationId(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.CountAsync(e => e.CorrelationId == id && e.Sent != null);
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

        if (entity.RetryCount is null || entity.RetryCount == 0)
        {
            entity.RetryCount = 1;
            entity.NextSchedule = _timeProvider.GetUtcNow().AddHours(1);
        }
        else
        {
            var pow = Math.Pow(2, entity.RetryCount.Value);
            entity.RetryCount++;
            var delay = _timeProvider.GetUtcNow().AddHours(pow);
            entity.NextSchedule = delay;
        }

        return Result.Ok();
    }
}
