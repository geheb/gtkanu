using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.Boats;

[Node("Mieten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,boatmanager")]
public class RentalsModel : PageModel
{
    private readonly Core.Repositories.Boats _boats;

    public BoatRentalDto[] Items { get; set; } = new BoatRentalDto[0];

    public string? BoatDetails { get; set; }

    public RentalsModel(Core.Repositories.Boats boats)
    {
        _boats = boats;
    }

    public async Task OnGetAsync(Guid id, int filter, CancellationToken cancellationToken)
    {
        var boat = await _boats.FindBoat(id, cancellationToken);
        if (boat is null)
        {
            ModelState.AddModelError(string.Empty, "Das Boot wurde nicht gefunden.");
            return;
        }

        BoatDetails = boat.FullDetails;

        Items = await _boats.GetRentals(id, filter == 0, cancellationToken);
    }

    public async Task<IActionResult> OnPostStopAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boats.StopRental(id, cancellationToken);
        return new JsonResult(result);
    }

    public async Task<IActionResult> OnPostCancelAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _boats.CancelRental(id, cancellationToken);
        return new JsonResult(result);
    }
}
