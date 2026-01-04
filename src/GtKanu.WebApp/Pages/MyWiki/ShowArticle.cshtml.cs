namespace GtKanu.WebApp.Pages.MyWiki;

using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Node("Wikiartikel", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public class ShowArticleModel : PageModel
{
    private IUnitOfWork _unitOfWork;

    public WikiArticleDto? Item { get; private set; }

    public ShowArticleModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        Item = await _unitOfWork.WikiArticles.Find(id, cancellationToken);
    }
}
