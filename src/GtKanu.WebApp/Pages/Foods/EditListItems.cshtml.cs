using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.Foods;

[Node("Einträge verwalten", FromPage = typeof(EditListModel))]
[Authorize(Roles = "administrator,treasurer")]
public class EditListItemsModel : PageModel
{
    private readonly Core.Repositories.Foods _foods;
    public FoodDto[] Foods { get; set; } = Array.Empty<FoodDto>();

    public string? ListDetails { get; set; } = "n.v.";

    public EditListItemsModel(Core.Repositories.Foods foods) => _foods = foods;

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
        var foodList = await _foods.Find(id, cancellationToken);
        if (foodList == null)
        {
            return;
        }
        var datetimeConverter = new GermanDateTimeConverter();
        ListDetails = foodList.Name + ", gültig ab " + datetimeConverter.ToDateTime(foodList.ValidFrom);
    }
}
