namespace GtKanu.Infrastructure.Database.Repositories;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

internal sealed class WikiArticleRepository : Repository<WikiArticle, WikiArticleDto>, IWikiArticles
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
