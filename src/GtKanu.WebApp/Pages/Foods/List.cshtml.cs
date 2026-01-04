using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Foods;

[Node("Getränke-/Speisen-/Spendenliste", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,treasurer")]
public class ListModel : PageModel
{
    private readonly IFoods _foods;
    public FoodListDto[] FoodLists { get; set; } = Array.Empty<FoodListDto>();

    public ListModel(IFoods foods)
    {
        _foods = foods;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        FoodLists = await _foods.GetFoodList(cancellationToken);
    }
}
