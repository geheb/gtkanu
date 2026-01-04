using GtKanu.Application.Converter;
using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Tryouts;

[Node("Anmeldungen zum Training", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,tripmanager")]
public class TryoutBookingsModel : PageModel
{
    private readonly ITryouts _tryouts;

    public string? TryoutDetails { get; private set; }
    public TryoutBookingDto[] Items { get; private set; } = [];

    public TryoutBookingsModel(ITryouts tryouts)
    {
        _tryouts = tryouts;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var tryout = await _tryouts.FindTryoutList(id, cancellationToken);
        if (tryout == null)
        {
            return;
        }

        var dc = new GermanDateTimeConverter();
        TryoutDetails = $"{tryout.Type} ({dc.ToDateTime(tryout.Date)}) @ {tryout.ContactName}";

        Items = await _tryouts.GetBookingList(id, cancellationToken);
    }

    public async Task<IActionResult> OnPostConfirmAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _tryouts.ConfirmBooking(id, cancellationToken);
        return new JsonResult(result);
    }

    public async Task<IActionResult> OnPostCancelAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _tryouts.CancelBooking(id, cancellationToken);
        return new JsonResult(result);
    }
}
