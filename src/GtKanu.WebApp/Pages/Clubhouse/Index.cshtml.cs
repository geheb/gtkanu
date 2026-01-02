using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.Clubhouse;

[Node("Vereinsheimbelegung", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,housemanager")]
public class IndexModel : PageModel
{
    private readonly Core.Repositories.Clubhouse _clubhouse;

    public ClubhouseBookingDto[] Items { get; set; } = [];

    public IndexModel(Core.Repositories.Clubhouse clubhouse)
    {
        _clubhouse = clubhouse;
    }

    public async Task OnGetAsync(int filter, CancellationToken cancellationToken)
    {
        var showExpired = filter == 1;
        Items = await _clubhouse.GetBookingList(showExpired, cancellationToken);
    }
}
