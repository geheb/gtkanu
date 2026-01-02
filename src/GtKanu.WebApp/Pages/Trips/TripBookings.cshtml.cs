namespace GtKanu.WebApp.Pages.Trips;

using GtKanu.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Anmeldungen an der Fahrt", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,tripmanager")]
public class TripBookingsModel : PageModel
{
    private readonly Trips _trips;
    public string? TripDetails { get; private set; }
    public TripBookingDto[] Items { get; private set; } = Array.Empty<TripBookingDto>();

    public TripBookingsModel(Trips trips)
    {
        _trips = trips;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var trip = await _trips.FindTripList(id, cancellationToken);
        if (trip == null)
        {
            return;
        }

        var dc = new GermanDateTimeConverter();
        TripDetails = dc.Format(trip.Start, trip.End) + " - " + trip.Target + " @ " + trip.ContactName;

        Items = await _trips.GetBookingList(id, cancellationToken);
    }

    public async Task<IActionResult> OnPostConfirmAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _trips.ConfirmBooking(id, cancellationToken);
        return new JsonResult(result);
    }

    public async Task<IActionResult> OnPostCancelAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _trips.CancelBooking(id, cancellationToken);
        return new JsonResult(result);
    }
}
