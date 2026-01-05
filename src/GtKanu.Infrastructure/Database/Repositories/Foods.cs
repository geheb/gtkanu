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
        var entities = await _dbContext.FoodLists
            .AsNoTracking()
            .Select(e => new 
            { 
                list = e, 
                count = e.Foods == null ? 0 : e.Foods.Count
            })
            .OrderByDescending(e => e.list.ValidFrom)
            .ToArrayAsync(cancellationToken);

        var dc = new GermanDateTimeConverter();

        return [.. entities.Select(e => e.list.ToDto(e.count, dc))];
    }

    public async Task<FoodDto[]> GetFoods(Guid foodListId, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Foods
            .AsNoTracking()
            .Where(e => e.FoodListId == foodListId)
            .OrderBy(e => e.Type).ThenBy(e => e.Name)
            .ToArrayAsync(cancellationToken);

        return [.. entities.Select(e => e.ToDto())];
    }

    public async Task<bool> DeleteFood(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Foods.FindAsync([id], cancellationToken);
        if (entity == null) return false;

        _dbContext.Remove(entity);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> Create(FoodListDto dto, CancellationToken cancellationToken)
    {
        var entity = new FoodList();
        entity.FromDto(dto);
        entity.Id = _dbContext.GeneratePk();

        _dbContext.Add(entity);

        dto.Id = entity.Id;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<FoodDto[]> GetLatestFoods(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var latestList = await _dbContext.FoodLists
            .AsNoTracking()
            .Where(e => e.ValidFrom <= now)
            .OrderByDescending(e => e.ValidFrom)
            .FirstOrDefaultAsync(cancellationToken);

        if (latestList == null)
        {
            return [];
        }

        var entities = await _dbContext.Foods
            .AsNoTracking()
            .Where(e => e.FoodListId == latestList.Id)
            .OrderBy(e => e.Type).ThenBy(e => e.Name)
            .ToArrayAsync(cancellationToken);

        return [.. entities.Select(e => e.ToDto())];
    }

    public async Task<bool> Create(FoodDto dto, CancellationToken cancellationToken)
    {
        var entity = new Food();
        entity.FromDto(dto);
        entity.Id = _dbContext.GeneratePk();

        _dbContext.Add(entity);

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<FoodListDto?> FindFoodList(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.FoodLists.FindAsync([id], cancellationToken);
        if (entity == null) return null;

        return entity.ToDto(null, new());
    }

    public async Task<bool> Update(FoodListDto dto, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.FoodLists.FindAsync([dto.Id], cancellationToken);
        if (entity == null) return false;

        var count = 0;
        if (entity.SetValue(e => e.Name, dto.Name)) count++;
        if (entity.SetValue(e => e.ValidFrom, dto.ValidFrom)) count++;

        if (count < 1) return true;

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
