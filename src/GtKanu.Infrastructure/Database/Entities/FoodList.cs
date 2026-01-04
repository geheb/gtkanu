using GtKanu.Application.Converter;
using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

internal sealed class FoodList
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset ValidFrom { get; set; }
    public ICollection<Food>? Foods { get; set; }

    internal FoodListDto ToDto(int? count, GermanDateTimeConverter dc) => new()
    {
        Id = Id,
        Name = Name,
        ValidFrom = dc.ToLocal(ValidFrom),
        Count = count
    };

    internal void FromDto(FoodListDto dto)
    {
        Id = Guid.Empty;
        Name = dto.Name?.Trim();
        ValidFrom = dto.ValidFrom;
    }
}
