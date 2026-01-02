namespace GtKanu.Ui.Pages.Tryouts;

using GtKanu.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;


[Node("Trainings", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,tripmanager")]
public class IndexModel : PageModel
{
    private readonly Tryouts _tryouts;

    public TryoutListDto[] Items { get; private set; } = Array.Empty<TryoutListDto>();

    public IndexModel(Tryouts tryouts)
    {
        _tryouts = tryouts;
    }

    public async Task OnGetAsync(int filter, CancellationToken cancellationToken)
    {
        var showExpired = filter == 1;
        Items = await _tryouts.GetTryoutList(showExpired, false, cancellationToken);
    }
}
