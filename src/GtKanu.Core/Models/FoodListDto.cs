using GtKanu.Core.Converter;
using GtKanu.Core.Entities;

namespace GtKanu.Core.Models
{
    public sealed class FoodListDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; } = null!;
        public DateTimeOffset ValidFrom { get; set; }
        public int? Count { get; set; }

        public FoodListDto()
        {
        }

        internal FoodListDto(FoodList foodList, int? count, GermanDateTimeConverter dc)
        {
            Id = foodList.Id;
            Name = foodList.Name;
            ValidFrom = dc.ToLocal(foodList.ValidFrom);
            Count = count;
        }

        internal FoodList CreateEntity()
        {
            return new FoodList
            {
                Id = Guid.Empty,
                Name = Name,
                ValidFrom = ValidFrom
            };
        }
    }
}
