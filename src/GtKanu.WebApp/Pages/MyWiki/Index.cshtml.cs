using GtKanu.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.Ui.Pages.MyWiki;

[Node("Mein Wiki", FromPage = typeof(Pages.IndexModel))]
[Authorize(Roles = "administrator,member")]
public class IndexModel : PageModel
{
    private readonly UnitOfWork _unitOfWork;

    public WikiArticleDto[] Items { get; set; } = [];

    public IndexModel(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.WikiArticles.GetAll(cancellationToken);
        Items = [.. items.OrderBy(e => e.Title)];
    }
}
