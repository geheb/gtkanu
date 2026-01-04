using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.MyMailings;

[Node("Meine Mailings", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member")]
public sealed class IndexModel : PageModel
{
    private readonly IUnitOfWork _unitOfWork;
    public MyMailingDto[] Items { get; set; } = [];

    public IndexModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.MyMailings.GetByUser(User.GetId(), cancellationToken);
        Items = [.. items.OrderByDescending(e => e.Created)];
    }
}
