namespace GtKanu.Application.Models;

public sealed class FoodDto
{
    public Guid Id { get; set; }
    public FoodType Type { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public Guid? FoodListId { get; set; }
}
