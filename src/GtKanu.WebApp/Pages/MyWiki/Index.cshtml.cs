using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.MyWiki;

[Node("Mein Wiki", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member")]
public class IndexModel : PageModel
{
    private readonly IUnitOfWork _unitOfWork;

    public WikiArticleDto[] Items { get; set; } = [];

    public IndexModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.WikiArticles.GetAll(cancellationToken);
        Items = [.. items.OrderBy(e => e.Title)];
    }
}
