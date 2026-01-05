using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Clubhouse;

[Node("Vereinsheimbelegung", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,housemanager")]
public class IndexModel : PageModel
{
    private readonly IClubhouseBookings _clubhouse;

    public ClubhouseBookingDto[] Items { get; set; } = [];

    public IndexModel(IClubhouseBookings clubhouse)
    {
        _clubhouse = clubhouse;
    }

    public async Task OnGetAsync(int filter, CancellationToken cancellationToken)
    {
        var showExpired = filter == 1;
        Items = await _clubhouse.GetAll(showExpired, cancellationToken);
    }
}
