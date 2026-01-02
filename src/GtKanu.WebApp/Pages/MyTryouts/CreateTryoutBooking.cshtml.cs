using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.MyTryouts;

[Node("Training buchen", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member,interested")]
public class CreateTryoutBookingModel : PageModel
{
    private readonly Core.Repositories.Tryouts _tryouts;

    public TryoutListDto[] Items { get; set; } = Array.Empty<TryoutListDto>();

    public CreateTryoutBookingModel(Core.Repositories.Tryouts tryouts)
    {
        _tryouts = tryouts;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Items = await _tryouts.GetTryoutList(false, User.IsInRole(Roles.Member), cancellationToken);
    }

    public async Task<IActionResult> OnPostCreateAsync(Guid id, string? name, CancellationToken cancellationToken)
    {
        var result = await _tryouts.CreateBooking(id, User.GetId(), name, cancellationToken);
        return new JsonResult(result.ToString());
    }
}
