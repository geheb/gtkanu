using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class Food
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Type { get; set; }
    public Guid? FoodListId { get; set; }
    public FoodList? FoodList { get; set; }
    public ICollection<Booking>? Bookings { get; set; }

    internal FoodDto ToDto() => new()
    {
        Id = Id,
        Type = (FoodType)Type,
        Name = Name,
        Price = Price,
        FoodListId = FoodListId
    };

    internal void FromDto(FoodDto dto)
    {
        Id = dto.Id;
        Type = (int)dto.Type;
        Name = dto.Name;
        Price = dto.Price;
        FoodListId = dto.FoodListId;
    }
}
