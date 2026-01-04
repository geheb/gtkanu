namespace GtKanu.Application.Models;

public sealed class FoodListDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset ValidFrom { get; set; }
    public int? Count { get; set; }
}
