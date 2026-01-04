namespace GtKanu.WebApp.Pages.MyFleet;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


[Node("Mein Fuhrpark", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member")]
public class IndexModel : PageModel
{
    private readonly IVehicles _vehicles;
    public VehicleBookingDto[] Items { get; set; } = [];

    public IndexModel(IVehicles vehicles)
    {
        _vehicles = vehicles;
    }

    public async Task OnGetAsync(int filter, CancellationToken cancellationToken)
    {
        var showExpired = filter == 2;
        var showMine = filter == 0;
        Items = await _vehicles.GetBookings(showExpired, showMine ? User.GetId() : null, cancellationToken);
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _vehicles.DeleteBooking(id, User.GetId(), cancellationToken);
        return new JsonResult(result);
    }
}
