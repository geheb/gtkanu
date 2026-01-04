namespace GtKanu.WebApp.Pages.MyTryouts;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Mein Training", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member,interested")]
public class IndexModel : PageModel
{
    private readonly ITryouts _tryouts;

    public MyTryoutListDto[] Items { get; set; } = Array.Empty<MyTryoutListDto>();

    public IndexModel(ITryouts tryouts) 
    {
        _tryouts = tryouts;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _tryouts.GetMyTryoutList(User.GetId(), User.IsInRole(Roles.Member), cancellationToken);
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _tryouts.DeleteBooking(id, User.GetId(), cancellationToken);
        return new JsonResult(result);
    }
}
