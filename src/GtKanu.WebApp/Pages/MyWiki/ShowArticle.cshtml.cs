namespace GtKanu.Ui.Pages.MyWiki;

using GtKanu.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Wikiartikel", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public class ShowArticleModel : PageModel
{
    private UnitOfWork _unitOfWork;

    public WikiArticleDto? Item { get; private set; }

    public ShowArticleModel(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        Item = await _unitOfWork.WikiArticles.Find(id, cancellationToken);
    }
}
