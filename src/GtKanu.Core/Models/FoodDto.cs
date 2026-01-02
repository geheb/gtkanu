using GtKanu.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtKanu.Core.Models
{
    public sealed class FoodDto
    {
        public Guid Id { get; set; }
        public FoodType Type { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public Guid? FoodListId { get; set; }

        public FoodDto()
        {
        }

        internal FoodDto(Food food)
        {
            Id = food.Id;
            Type = (FoodType)food.Type;
            Name = food.Name;
            Price = food.Price;
            FoodListId = food.FoodListId;
        }

        internal Food CreateEntity()
        {
            return new Food
            {
                Id = Id,
                Type = (int)Type,
                Name = Name,
                Price = Price, 
                FoodListId = FoodListId
            };
        }
    }
}
