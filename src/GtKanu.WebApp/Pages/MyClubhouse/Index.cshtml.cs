using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.MyClubhouse;

[Node("Vereinsheimbelegung", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member")]
public class IndexModel : PageModel
{
    private readonly IClubhouse _clubhouse;

    public ClubhouseBookingDto[] Items { get; set; } = [];

    public IndexModel(IClubhouse clubhouse)
    {
        _clubhouse = clubhouse;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _clubhouse.GetBookingList(false, cancellationToken);
    }
}
