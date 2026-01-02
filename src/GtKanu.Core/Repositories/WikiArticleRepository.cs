namespace GtKanu.Core.Repositories;

using GtKanu.Core.Entities;
using GtKanu.Core.Models;
using Microsoft.EntityFrameworkCore;

public sealed class WikiArticleRepository : Repository<WikiArticle, WikiArticleDto>
{
    protected override IQueryable<WikiArticle> GetBaseQuery() => _dbSet.AsNoTracking().Include(e => e.User);

    public WikiArticleRepository(TimeProvider timeProvider, DbSet<WikiArticle> dbSet)
        : base(timeProvider, dbSet)
    {
    }

    public async Task<bool> FindIdentifier(string identifier, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(e => e.Identifier == identifier, cancellationToken);
    }
}
