using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.MyClubhouse;

[Node("Vereinsheimbelegung", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member")]
public class IndexModel : PageModel
{
    private readonly Core.Repositories.Clubhouse _clubhouse;

    public ClubhouseBookingDto[] Items { get; set; } = [];

    public IndexModel(Core.Repositories.Clubhouse clubhouse)
    {
        _clubhouse = clubhouse;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _clubhouse.GetBookingList(false, cancellationToken);
    }
}
