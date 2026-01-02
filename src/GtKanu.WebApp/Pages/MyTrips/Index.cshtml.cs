namespace GtKanu.Ui.Pages.MyTrips;

using GtKanu.Core.Repositories;
using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Mein Fahrtenplan", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member")]
public class IndexModel : PageModel
{
    private readonly Trips _trips;

    public MyTripListDto[] Items { get; set; } = Array.Empty<MyTripListDto>();

    public IndexModel(Trips trips)
    {
        _trips = trips;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _trips.GetMyTripList(User.GetId(), cancellationToken);
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _trips.DeleteBooking(id, User.GetId(), cancellationToken);
        return new JsonResult(result);
    }
}
