using FluentResults;
using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace GtKanu.Infrastructure.Database.Repositories;

internal class Repository<TEntity, TModel>
    where TModel : struct, IDto
    where TEntity : class, IEntity, IDtoMapper<TModel>, new()
{
    protected readonly TimeProvider _timeProvider;
    protected readonly DbSet<TEntity> _dbSet;

    protected Result NotFound => Result.Fail("Datensatz nicht gefunden.");

    protected virtual IQueryable<TEntity> GetBaseQuery() => _dbSet.AsNoTracking();

    public Repository(TimeProvider timeProvider, DbSet<TEntity> dbSet)
    {
        _timeProvider = timeProvider;
        _dbSet = dbSet;
    }

    public void Create(TModel model)
    {
        var entity = new TEntity();
        entity.FromDto(model);
        entity.Id = Guid.CreateVersion7();
        entity.Created = _timeProvider.GetUtcNow();
        _dbSet.Add(entity);
    }

    public async Task<TModel[]> GetAll(CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();
        return [.. (await GetBaseQuery().ToArrayAsync(cancellationToken)).Select(e => e.ToDto(dc))] ;
    }

    public async Task<TModel[]> Get(Guid[] ids, CancellationToken cancellationToken)
    {
        var dc = new GermanDateTimeConverter();
        var result = new List<TModel>(ids.Length);
        foreach (var chunk in ids.Chunk(100))
        {
            var entities = await GetBaseQuery()
                .Where(e => chunk.Contains(e.Id))
                .ToArrayAsync(cancellationToken);

            result.AddRange(entities.Select(e => e.ToDto(dc)));
        }

        return [.. result];
    }

    public async Task<TModel?> Find(Guid id, CancellationToken cancellationToken) =>
        (await GetBaseQuery().FirstOrDefaultAsync(e => e.Id == id, cancellationToken))?.ToDto(new());

    public async Task<Result> Update(TModel model, CancellationToken cancellationToken)
    {
        var entity = await _dbSet.FindAsync([model.Id], cancellationToken);
        if (entity is null)
        {
            return NotFound;
        }
        entity.FromDto(model);
        entity.Updated = _timeProvider.GetUtcNow();
        return Result.Ok();
    }

    public async Task<Result> Delete(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbSet.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound;
        }
        _dbSet.Remove(entity);
        return Result.Ok();
    }
}
