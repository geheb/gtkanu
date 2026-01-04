namespace GtKanu.WebApp.Pages.Trips;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Fahrtenplan", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,tripmanager")]
public class IndexModel : PageModel
{
    private readonly ITrips _trips;

    public TripListDto[] Items { get; set; } = [];

    public IndexModel(ITrips trips)
    {
        _trips = trips;
    }

    public async Task OnGetAsync(int filter, CancellationToken cancellationToken)
    {
        var showExpired = filter == 1;
        Items = await _trips.GetTripList(showExpired, cancellationToken);
    }
}
