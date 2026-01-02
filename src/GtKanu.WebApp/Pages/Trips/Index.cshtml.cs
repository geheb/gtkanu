namespace GtKanu.Ui.Pages.Trips;

using GtKanu.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Fahrtenplan", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,tripmanager")]
public class IndexModel : PageModel
{
    private readonly Trips _trips;

    public TripListDto[] Items { get; set; } = Array.Empty<TripListDto>();

    public IndexModel(Trips trips)
    {
        _trips = trips;
    }

    public async Task OnGetAsync(int filter, CancellationToken cancellationToken)
    {
        var showExpired = filter == 1;
        Items = await _trips.GetTripList(showExpired, cancellationToken);
    }
}
