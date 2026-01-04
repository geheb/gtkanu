namespace GtKanu.WebApp.Pages.MyTrips;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("An der Fahrt anmelden", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public class CreateTripBookingModel : PageModel
{
    private readonly ITrips _trips;

    public TripListDto[] Items { get; set; } = [];

    public CreateTripBookingModel(ITrips trips)
    {
        _trips = trips;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _trips.GetTripList(false, cancellationToken);
    }

    public async Task<IActionResult> OnPostCreateAsync(Guid id, string? name, CancellationToken cancellationToken)
    {
        var result = await _trips.CreateBooking(id, User.GetId(), name, cancellationToken);
        return new JsonResult(result.ToString());
    }
}
