using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Foods;

[Node("Einträge verwalten", FromPage = typeof(EditListModel))]
[Authorize(Roles = "administrator,treasurer")]
public class EditListItemsModel : PageModel
{
    private readonly IFoods _foods;
    public FoodDto[] Foods { get; set; } = [];

    public string? ListDetails { get; set; } = "n.v.";

    public EditListItemsModel(IFoods foods) => _foods = foods;

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        await UpdateListDetails(id, cancellationToken);

        Foods = await _foods.GetFoods(id, cancellationToken);
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid foodId, CancellationToken cancellationToken)
    {
        var result = await _foods.DeleteFood(foodId, cancellationToken);
        return new JsonResult(result);
    }

    private async Task UpdateListDetails(Guid id, CancellationToken cancellationToken)
    {
        var foodList = await _foods.FindFoodList(id, cancellationToken);
        if (foodList == null)
        {
            return;
        }
        var datetimeConverter = new GermanDateTimeConverter();
        ListDetails = foodList.Name + ", gültig ab " + datetimeConverter.ToDateTime(foodList.ValidFrom);
    }
}
