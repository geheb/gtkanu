using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.Trips;

[AllowAnonymous]
public class PublicModel : PageModel
{
    private readonly Core.Repositories.Trips _trips;

    public PublicTripDto[] Items { get; set; } = [];

    public PublicModel(Core.Repositories.Trips trips)
    {
        _trips = trips;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _trips.GetPublicTrips(cancellationToken);
    }

    public async Task<IActionResult> OnGetIcsAsync(CancellationToken cancellationToken)
    {
        var ics = await _trips.GetPublicTripsAsIcs(cancellationToken);
        return Content(ics, "text/calendar");
    }
}
