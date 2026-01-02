using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.MyBoats;

[Node("Mieten", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public class RentalsModel : PageModel
{
    private readonly Core.Repositories.Boats _boats;

    public MyBoatRentalListDto[] Items { get; set; } = Array.Empty<MyBoatRentalListDto>();

    public string? BoatDetails { get; set; }

    public RentalsModel(Core.Repositories.Boats boats)
    {
        _boats = boats;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var boat = await _boats.FindBoat(id, cancellationToken);
        if (boat is null)
        {
            ModelState.AddModelError(string.Empty, "Das Boot wurde nicht gefunden.");
            return;
        }

        BoatDetails = boat.FullDetails;

        Items = await _boats.GetMyRentals(User.GetId(), cancellationToken);
    }
}
