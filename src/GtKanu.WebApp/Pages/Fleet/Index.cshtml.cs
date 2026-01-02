namespace GtKanu.Ui.Pages.Fleet;

using GtKanu.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Fuhrpark", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,fleetmanager")]
public class IndexModel : PageModel
{
    private readonly Vehicles _vehicles;

    public VehicleBookingDto[] Items { get; set; } = Array.Empty<VehicleBookingDto>();

    public IndexModel(Vehicles vehicles)
    {
        _vehicles = vehicles;
    }

    public async Task OnGetAsync(int filter, CancellationToken cancellationToken)
    {
        bool showExpired = filter == 1;
        Items = await _vehicles.GetBookings(showExpired, null, cancellationToken);
    }

    public async Task<IActionResult> OnPostConfirmAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _vehicles.ConfirmBooking(id, cancellationToken);
        return new JsonResult(result);
    }

    public async Task<IActionResult> OnPostCancelAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _vehicles.CancelBooking(id, cancellationToken);
        return new JsonResult(result);
    }
}
