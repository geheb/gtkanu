using GtKanu.Core.Converter;
using GtKanu.Core.Database;
using GtKanu.Core.Entities;
using GtKanu.Core.Extensions;
using GtKanu.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GtKanu.Core.Repositories;

public class Foods
{
    private UuidPkGenerator _pkGenerator = new();
    private readonly AppDbContext _dbContext;

    public Foods(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FoodListDto[]> GetFoodList(CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<FoodList>();
        var entities = await dbSet
            .AsNoTracking()
            .Select(e => new { list = e, count = e.Foods != null ? e.Foods.Count : 0 })
            .OrderByDescending(e => e.list.ValidFrom)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return entities.Select(e => new FoodListDto(e.list, e.count, dc)).ToArray();
    }

    public async Task<FoodDto[]> GetFoods(Guid foodListId, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Food>();

        var entities = await dbSet
            .AsNoTracking()
            .Where(e => e.FoodListId == foodListId)
            .OrderBy(e => e.Type).ThenBy(e => e.Name)
            .ToArrayAsync(cancellationToken);

        return entities.Select(e => new FoodDto(e)).ToArray();
    }

    public async Task<bool> DeleteFood(Guid id, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Food>();

        var entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return false;
        dbSet.Remove(entity);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> Create(FoodListDto dto, CancellationToken cancellationToken)
    {
        var entity = dto.CreateEntity();
        entity.Id = _pkGenerator.Generate();

        var dbSet = _dbContext.Set<FoodList>();

        await dbSet.AddAsync(entity, cancellationToken);

        dto.Id = entity.Id;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<FoodDto[]> GetLatestFoods(CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<FoodList>();

        var latestList = await dbSet
            .AsNoTracking()
            .Where(e => e.ValidFrom <= DateTimeOffset.UtcNow)
            .OrderByDescending(e => e.ValidFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestList == null)
        {
            return Array.Empty<FoodDto>();
        }

        var dbSetFood = _dbContext.Set<Food>();

        var entities = await dbSetFood
            .AsNoTracking()
            .Where(e => e.FoodListId == latestList.Id)
            .OrderBy(e => e.Type).ThenBy(e => e.Name)
            .ToArrayAsync(cancellationToken);

        return entities.Select(e => new FoodDto(e)).ToArray();
    }

    public async Task<bool> Create(FoodDto dto, CancellationToken cancellationToken)
    {
        var entity = dto.CreateEntity();
        entity.Id = _pkGenerator.Generate();

        var dbSet = _dbContext.Set<Food>();

        await dbSet.AddAsync(entity, cancellationToken);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<FoodListDto?> Find(Guid id, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<FoodList>();

        var entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return null;

        return new FoodListDto(entity, null, new GermanDateTimeConverter());
    }

    public async Task<bool> Update(FoodListDto dto, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<FoodList>();

        var entity = await dbSet.FindAsync(new object[] { dto.Id }, cancellationToken);
        if (entity == null) return false;

        var count = 0;
        if (entity.SetValue(e => e.Name, dto.Name)) count++;
        if (entity.SetValue(e => e.ValidFrom, dto.ValidFrom)) count++;

        if (count < 1) return true;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
