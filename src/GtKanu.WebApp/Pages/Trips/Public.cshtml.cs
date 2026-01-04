using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Trips;

[AllowAnonymous]
public class PublicModel : PageModel
{
    private readonly ITrips _trips;

    public PublicTripDto[] Items { get; set; } = [];

    public PublicModel(ITrips trips)
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
