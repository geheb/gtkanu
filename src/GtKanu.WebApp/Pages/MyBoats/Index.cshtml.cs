using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.MyBoats;


[Node("Mein Bootslager", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member")]
public class IndexModel : PageModel
{
    private readonly Core.Repositories.Boats _boats;

    public BoatRentalListDto[] Items { get; set; } = Array.Empty<BoatRentalListDto>();

    public IndexModel(Core.Repositories.Boats boats)
    {
        _boats = boats;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _boats.GetMyRentalList(User.GetId(), cancellationToken);
    }
}
