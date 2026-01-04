using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.Mailings;

[Node("Mailings", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,mailingmanager")]
public class IndexModel : PageModel
{
    private readonly IUnitOfWork _unitOfWork;

    public MailingDto[] Items { get; set; } = [];

    public IndexModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Mailings.GetAll(cancellationToken);
        Items = [.. items.OrderByDescending(e => e.LastUpdate)];
    }
}
