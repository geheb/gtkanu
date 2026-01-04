using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GtKanu.Infrastructure.Database.Repositories;

internal sealed class Foods : IFoods
{
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

        return entities.Select(e => e.list.ToDto(e.count, dc)).ToArray();
    }

    public async Task<FoodDto[]> GetFoods(Guid foodListId, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Food>();

        var entities = await dbSet
            .AsNoTracking()
            .Where(e => e.FoodListId == foodListId)
            .OrderBy(e => e.Type).ThenBy(e => e.Name)
            .ToArrayAsync(cancellationToken);

        return entities.Select(e => e.ToDto()).ToArray();
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
        var entity = new FoodList();
        entity.FromDto(dto);
        entity.Id = _dbContext.GeneratePk();

        var dbSet = _dbContext.Set<FoodList>();

        dbSet.Add(entity);

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

        return entities.Select(e => e.ToDto()).ToArray();
    }

    public async Task<bool> Create(FoodDto dto, CancellationToken cancellationToken)
    {
        var entity = new Food();
        entity.FromDto(dto);
        entity.Id = _dbContext.GeneratePk();

        var dbSet = _dbContext.Set<Food>();

        dbSet.Add(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<FoodListDto?> Find(Guid id, CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<FoodList>();

        var entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null) return null;

        return entity.ToDto(null, new());
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
