namespace GtKanu.Infrastructure.Tests.Database;

using GtKanu.Application.Models;
using GtKanu.Infrastructure.Database.Entities;
using GtKanu.Infrastructure.Database.Repositories;

public class FoodsRepositoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task CreateFoodList_ShouldAddList()
    {
        using var context = _factory.CreateContext();
        var foods = new Foods(context);

        var dto = new FoodListDto
        {
            Id = Guid.NewGuid(),
            Name = "Summer Menu",
            ValidFrom = DateTimeOffset.UtcNow
        };

        var result = await foods.Create(dto, CancellationToken.None);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetFoodList_ShouldReturnListsOrderedByValidFrom()
    {
        using var context = _factory.CreateContext();
        var foods = new Foods(context);

        var now = DateTimeOffset.UtcNow;
        await foods.Create(new FoodListDto { Id = Guid.NewGuid(), Name = "Old", ValidFrom = now.AddDays(-10) }, CancellationToken.None);
        await foods.Create(new FoodListDto { Id = Guid.NewGuid(), Name = "New", ValidFrom = now.AddDays(-1) }, CancellationToken.None);

        var list = await foods.GetFoodList(CancellationToken.None);
        list.Should().HaveCount(2);
        list[0].Name.Should().Be("New");
        list[1].Name.Should().Be("Old");
    }

    [Fact]
    public async Task GetLatestFoods_ShouldReturnFoodsFromLatestList()
    {
        using var context = _factory.CreateContext();
        var foods = new Foods(context);

        var now = DateTimeOffset.UtcNow;
        var listId = Guid.NewGuid();
        context.FoodLists.Add(new FoodList { Id = listId, Name = "Latest", ValidFrom = now.AddDays(-1) });
        context.Foods.Add(new Food { Id = Guid.NewGuid(), Name = "Pizza", Price = 8.50m, Type = (int)FoodType.Dish, FoodListId = listId });
        context.Foods.Add(new Food { Id = Guid.NewGuid(), Name = "Cola", Price = 2.50m, Type = (int)FoodType.Drink, FoodListId = listId });
        await context.SaveChangesAsync();

        var result = await foods.GetLatestFoods(CancellationToken.None);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetLatestFoods_NoList_ShouldReturnEmpty()
    {
        using var context = _factory.CreateContext();
        var foods = new Foods(context);

        var result = await foods.GetLatestFoods(CancellationToken.None);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateFood_ShouldAddFood()
    {
        using var context = _factory.CreateContext();
        var foods = new Foods(context);

        var dto = new FoodDto
        {
            Id = Guid.NewGuid(),
            Name = "Burger",
            Price = 10.00m,
            Type = FoodType.Dish
        };

        var result = await foods.Create(dto, CancellationToken.None);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteFood_ExistingFood_ShouldReturnTrue()
    {
        using var context = _factory.CreateContext();
        var foods = new Foods(context);

        var id = Guid.NewGuid();
        context.Foods.Add(new Food { Id = id, Name = "Del", Price = 1m, Type = (int)FoodType.Drink });
        await context.SaveChangesAsync();

        var result = await foods.DeleteFood(id, CancellationToken.None);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteFood_NonExistingFood_ShouldReturnFalse()
    {
        using var context = _factory.CreateContext();
        var foods = new Foods(context);

        var result = await foods.DeleteFood(Guid.NewGuid(), CancellationToken.None);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetFoods_ShouldReturnFoodsForList()
    {
        using var context = _factory.CreateContext();
        var foods = new Foods(context);

        var listId = Guid.NewGuid();
        context.FoodLists.Add(new FoodList { Id = listId, Name = "L", ValidFrom = DateTimeOffset.UtcNow });
        context.Foods.Add(new Food { Id = Guid.NewGuid(), Name = "A", Price = 1m, Type = (int)FoodType.Dish, FoodListId = listId });
        context.Foods.Add(new Food { Id = Guid.NewGuid(), Name = "B", Price = 1m, Type = (int)FoodType.Drink, FoodListId = listId });
        await context.SaveChangesAsync();

        var result = await foods.GetFoods(listId, CancellationToken.None);
        result.Should().HaveCount(2);
    }
}
