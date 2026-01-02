using GtKanu.Core.Repositories;
using GtKanu.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.MyMailings;

[Node("Meine Mailings", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member")]
public sealed class IndexModel : PageModel
{
    private readonly UnitOfWork _unitOfWork;
    public MyMailingDto[] Items { get; set; } = [];

    public IndexModel(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.MyMailings.GetByUser(User.GetId(), cancellationToken);
        Items = [.. items.OrderByDescending(e => e.Created)];
    }
}
