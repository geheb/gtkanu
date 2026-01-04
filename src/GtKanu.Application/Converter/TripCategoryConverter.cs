using GtKanu.Application.Models;

namespace GtKanu.Application.Converter;

public sealed class TripCategoryConverter
{
    public string CategoryToClass(TripCategory category) =>
        category switch
        {
            TripCategory.Junior => "has-text-info",
            TripCategory.JuniorAdvanced => "has-text-success",
            TripCategory.Advanced => "has-text-danger",
            TripCategory.YoungPeople => "has-text-warning",
            _ => string.Empty
        };

    public string CategoryToName(TripCategory category) =>
        category switch
        {
            TripCategory.Junior => "A",
            TripCategory.JuniorAdvanced => "FA",
            TripCategory.Advanced => "F",
            TripCategory.YoungPeople => "J",
            _ => string.Empty
        };
}
