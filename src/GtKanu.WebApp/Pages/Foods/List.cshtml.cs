using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.Foods
{
    [Node("Getränke-/Speisen-/Spendenliste", FromPage = typeof(IndexModel))]
    [Authorize(Roles = "administrator,treasurer")]
    public class ListModel : PageModel
    {
        private readonly Core.Repositories.Foods _foods;
        public FoodListDto[] FoodLists { get; set; } = Array.Empty<FoodListDto>();

        public ListModel(Core.Repositories.Foods foods)
        {
            _foods = foods;
        }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            FoodLists = await _foods.GetFoodList(cancellationToken);
        }
    }
}
