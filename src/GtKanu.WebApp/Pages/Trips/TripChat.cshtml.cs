namespace GtKanu.WebApp.Pages.Trips;

using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Chat zur Fahrt", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,tripmanager")]
public class TripChatModel : PageModel
{
    private readonly ITrips _trips;

    public string? TripDetails { get; set; }
    public TripChatDto[] Items { get; set; } = [];
    public bool IsDisabled { get; set; }

    [BindProperty]
    public string? Message { get; set; }

    public TripChatModel(ITrips trips)
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
