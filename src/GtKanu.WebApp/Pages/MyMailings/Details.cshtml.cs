using GtKanu.Application.Models;
using GtKanu.Application.Repositories;
using GtKanu.Infrastructure.AspNetCore.Routing;
using GtKanu.Infrastructure.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GtKanu.WebApp.Pages.MyMailings;

[Node("Mailing", FromPage = typeof(IndexModel))]
[Authorize(Roles = "administrator,member")]
public sealed class DetailsModel : PageModel
{
    private IUnitOfWork _unitOfWork;

    public MyMailingDto? Item { get; set; }

    public DetailsModel(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.MyMailings.Find(id, cancellationToken);
        if (item?.UserId == User.GetId())
        {
            await _unitOfWork.MyMailings.UpdateHasRead(id, cancellationToken);
            await _unitOfWork.Save(cancellationToken);
            Item = item;
        }
    }
}
