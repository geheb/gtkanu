using FluentResults;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace GtKanu.Infrastructure.Database.Repositories;

internal sealed class MailingRepository : Repository<Mailing, MailingDto>, IMailings
{
    public MailingRepository(TimeProvider timeProvider, DbSet<Mailing> dbSet) 
        : base(timeProvider, dbSet)
    {
    }

    public async Task<Result> UpdateClosed(Guid id, int emailCount, CancellationToken cancellationToken)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound;
        }

        entity.Updated = _timeProvider.GetUtcNow();
        entity.IsClosed = true;
        entity.EmailCount = emailCount;

        return Result.Ok();
    }
}
