using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.MyBoats;


[Node("Mein Bootslager", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member")]
public class IndexModel : PageModel
{
    private readonly IBoats _boats;

    public BoatRentalListDto[] Items { get; set; } = [];

    public IndexModel(IBoats boats)
    {
        _boats = boats;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _boats.GetMyRentalList(User.GetId(), cancellationToken);
    }
}
