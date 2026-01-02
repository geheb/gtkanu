namespace GtKanu.Ui.Pages.MyTrips;

using GtKanu.Core.Repositories;
using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Chat zur Fahrt", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public class TripChatModel : PageModel
{
    private readonly Trips _trips;

    public string? TripDetails { get; set; }
    public TripChatDto[] Items { get; set; } = Array.Empty<TripChatDto>();
    public bool IsDisabled { get; set; }

    [BindProperty]
    public string? Message { get; set; }

    public TripChatModel(Trips trips)
    {
        _trips = trips;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var trip = await _trips.FindTripList(id, cancellationToken);
        if (trip == null)
        {
            IsDisabled = true;
            return;
        }

        IsDisabled = trip.IsExpired;

        var dc = new GermanDateTimeConverter();
        TripDetails = dc.Format(trip.Start, trip.End) + " - " + trip.Target + " @ " + trip.ContactName;

        Items = await _trips.GetChat(id, User.GetId(), cancellationToken);
    }

    public async Task<IActionResult> OnPostMessageAsync(Guid id, string? message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message) || message.Length > 256) return new JsonResult(false);
        var result = await _trips.CreateChatMessage(id, User.GetId(), message, cancellationToken);
        return new JsonResult(result);
    }
}
