using GtKanu.Application.Models;

namespace GtKanu.Application.Repositories;

public interface IFoods
{
    Task<FoodListDto[]> GetFoodList(CancellationToken cancellationToken);
    Task<FoodDto[]> GetFoods(Guid foodListId, CancellationToken cancellationToken);
    Task<bool> DeleteFood(Guid id, CancellationToken cancellationToken);
    Task<bool> Create(FoodListDto dto, CancellationToken cancellationToken);
    Task<FoodDto[]> GetLatestFoods(CancellationToken cancellationToken);
    Task<bool> Create(FoodDto dto, CancellationToken cancellationToken);
    Task<FoodListDto?> Find(Guid id, CancellationToken cancellationToken);
    Task<bool> Update(FoodListDto dto, CancellationToken cancellationToken);
}
